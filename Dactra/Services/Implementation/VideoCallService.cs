using Dactra.DTOs.VideoCallDTOs;

namespace Dactra.Services.Implementation
{
    public class VideoCallService : IVideoCallService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<VideoCallHub> _hub;
        private readonly IConfiguration _config;

        public VideoCallService(
            ApplicationDbContext context,
            IHubContext<VideoCallHub> hub,
            IConfiguration config)
        {
            _context = context;
            _hub = hub;
            _config = config;
        }

        public async Task<JoinRoomResponseDto> JoinRoomAsync(int appointmentId, string userId)
        {
            var appointment = await _context.PatientAppointments
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                        .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId)
                ?? throw new Exception("Appointment not found");

            var isDoctor = appointment.Slot.Doctor.UserId == userId;
            var isPatient = appointment.Patient.UserId == userId;

            if (!isDoctor && !isPatient)
                throw new UnauthorizedAccessException("Not authorized for this appointment");

            if (appointment.Status != AppointmentStatus.Confirmed)
                throw new Exception($"Appointment is {appointment.Status}, cannot start call");

            if (appointment.Slot.SlotType != SlotType.Online)
                throw new Exception("This is an in-person appointment, no video call needed");

            var session = await _context.VideoCallSessions
                .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId);

            if (session == null)
            {
                session = new VideoCallSession
                {
                    AppointmentId = appointmentId,
                    RoomName = GenerateRoomName(appointmentId),
                    Status = VideoCallStatus.Waiting,
                    CreatedAtUtc = DateTime.UtcNow
                };
                _context.VideoCallSessions.Add(session);
                await _context.SaveChangesAsync();
            }

            var now = DateTime.UtcNow.ToString("o");

            if (isDoctor && session.DoctorJoinedAt == null) session.DoctorJoinedAt = now;
            if (isPatient && session.PatientJoinedAt == null) session.PatientJoinedAt = now;

            if (session.DoctorJoinedAt != null && session.PatientJoinedAt != null
                && session.Status == VideoCallStatus.Waiting)
            {
                session.Status = VideoCallStatus.Active;
                session.StartedAtUtc = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            var notifyGroup = isDoctor
                ? $"Patient_{appointment.PatientId}"
                : $"Doctor_{appointment.Slot.DoctorId}";

            await _hub.Clients.Group(notifyGroup)
                .SendAsync("ParticipantJoined", new
                {
                    AppointmentId = appointmentId,
                    RoomName = session.RoomName,
                    JoinedAs = isDoctor ? "Doctor" : "Patient"
                });

            var user = isDoctor ? appointment.Slot.Doctor.User : appointment.Patient.User;
            var displayName = isDoctor
                ? $"Dr. {appointment.Slot.Doctor.FirstName} {appointment.Slot.Doctor.LastName}"
                : $"{appointment.Patient.FirstName} {appointment.Patient.LastName}";
            var role = isDoctor ? "moderator" : "participant";

            var jitsiToken = GenerateJitsiToken(
                roomName: session.RoomName,
                userId: userId,
                displayName: displayName,
                role: role
            );

            var appId = _config["Jitsi:AppId"];
            var domain = _config["Jitsi:Domain"];

            return new JoinRoomResponseDto(
                RoomName: $"{appId}/{session.RoomName}",
                JitsiToken: jitsiToken,
                JitsiDomain: domain,
                DisplayName: displayName,
                Role: role,
                SessionId: session.Id
            );
        }

        public async Task<RoomStatusDto> GetRoomStatusAsync(int appointmentId)
        {
            var session = await _context.VideoCallSessions
                .Include(s => s.Appointment)
                .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId);

            if (session == null)
                return new RoomStatusDto(0, "", VideoCallStatus.Waiting, false, false, null, 0);

            return new RoomStatusDto(
                SessionId: session.Id,
                RoomName: session.RoomName,
                Status: session.Status,
                IsDoctorOnline: session.DoctorJoinedAt != null,
                IsPatientOnline: session.PatientJoinedAt != null,
                StartedAtUtc: session.StartedAtUtc,
                PatientId: session.Appointment.PatientId
            );
        }

        public async Task EndCallAsync(int appointmentId, string userId)
        {
            var session = await _context.VideoCallSessions
                .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId)
                ?? throw new Exception("Session not found");

            session.Status = VideoCallStatus.Ended;
            session.EndedAtUtc = DateTime.UtcNow;

            var appointment = await _context.PatientAppointments
                .FindAsync(appointmentId);

            if (appointment != null)
                appointment.Status = AppointmentStatus.Completed;

            await _context.SaveChangesAsync();

            await _hub.Clients.Group($"Appointment_{appointmentId}")
                .SendAsync("CallEnded", new { AppointmentId = appointmentId });
        }


        private static string GenerateRoomName(int appointmentId)
        {
            var suffix = Guid.NewGuid().ToString("N")[..8];
            return $"dactra-apt-{appointmentId}-{suffix}";
        }

        private string GenerateJitsiToken(string roomName, string userId, string displayName, string role)
        {
            var appId = _config["Jitsi:AppId"];
            var keyId = _config["Jitsi:KeyId"];
            var privateKeyText = File.ReadAllText(_config["Jitsi:PrivateKeyPath"]);
            var expMinutes = int.Parse(_config["Jitsi:TokenExpiryMinutes"] ?? "60");

            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyText);
            var securityKey = new RsaSecurityKey(rsa) { KeyId = keyId };
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

            var isModerator = role == "moderator";

            var context = JsonSerializer.Serialize(new
            {
                user = new
                {
                    id = userId,
                    name = displayName,
                    moderator = isModerator,
                    avatar = ""
                },
                features = new
                {
                    recording = false,
                    outbound_call = false,
                    transcription = false,
                    livestreaming = false
                }
            });

            var claims = new[]
            {
        new Claim("iss", "chat"),
        new Claim("aud", "jitsi"),
        new Claim("sub", appId),
        new Claim("room", "*"),
        new Claim("context", context),
    };

            var handler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expMinutes),
                signingCredentials: credentials
            );

            token.Header["kid"] = keyId;

            return handler.WriteToken(token);
        }
    }
}
