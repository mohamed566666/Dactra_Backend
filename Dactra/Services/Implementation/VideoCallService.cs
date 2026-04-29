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

            return new JoinRoomResponseDto(
                RoomName: session.RoomName,
                JitsiToken: jitsiToken,
                JitsiDomain: _config["Jitsi:Domain"] ?? "meet.jit.si",
                DisplayName: displayName,
                Role: role,
                SessionId: session.Id
            );
        }

        public async Task<RoomStatusDto> GetRoomStatusAsync(int appointmentId)
        {
            var session = await _context.VideoCallSessions
                .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId);

            if (session == null)
                return new RoomStatusDto(0, string.Empty, VideoCallStatus.Waiting, false, false, null);

            return new RoomStatusDto(
                SessionId: session.Id,
                RoomName: session.RoomName,
                Status: session.Status,
                IsDoctorOnline: session.DoctorJoinedAt != null,
                IsPatientOnline: session.PatientJoinedAt != null,
                StartedAtUtc: session.StartedAtUtc
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

        private string GenerateJitsiToken(
            string roomName,
            string userId,
            string displayName,
            string role)
        {
            var secret = _config["Jitsi:Secret"];
            var appId = _config["Jitsi:AppId"];
            var expiry = int.Parse(_config["Jitsi:TokenExpiryMinutes"] ?? "60");
            var domain = _config["Jitsi:Domain"] ?? "meet.jit.si";

            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(appId))
                return string.Empty;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("room",    roomName),
                new Claim("sub",     domain),
                new Claim("iss",     appId),
                new Claim("aud",     appId),
                new Claim("context", System.Text.Json.JsonSerializer.Serialize(new
                {
                    user = new
                    {
                        id           = userId,
                        name         = displayName,
                        moderator    = role == "moderator"
                    }
                }))
            };

            var token = new JwtSecurityToken(
                issuer: appId,
                audience: appId,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
