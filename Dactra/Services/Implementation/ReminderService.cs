
namespace Dactra.Services.Implementation
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IAppointmentReminderService _appointmentReminderService;

        public ReminderService(ApplicationDbContext context, INotificationService notificationService, IAppointmentReminderService appointmentReminderService)
        {
            _context = context;
            _notificationService = notificationService;
            _appointmentReminderService = appointmentReminderService;
        }


        public async Task SendReminder(int appointmentId)
        {
            try
            {
                var appt = await _context.PatientAppointments
                 .Include(a => a.Slot)
                 .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appt == null || appt.IsReminderSent)
                {
                    return;
                }
                var patientTokens = await _context.NotificationSubscriptions
                       .Where(x => x.PatientId == appt.PatientId.ToString() && x.IsActive)
                       .Select(x => x.FcmToken)
                       .ToListAsync();


                var doctorUserId = await _context.Doctors.Where(d => d.Id == appt.Slot.DoctorId)
                    .Select(d => d.UserId)
                    .FirstOrDefaultAsync();

                var doctorTokens = await _context.NotificationSubscriptions
                    .Where(x => x.PatientId == doctorUserId && x.IsActive)
                    .Select(x => x.FcmToken)
                    .ToListAsync();

                var patientUserId = await _context.Patients.Where(p => p.Id == appt.PatientId)
                    .Select(p => p.UserId)
                    .FirstOrDefaultAsync();
                var utcTime = appt.Slot.SlotDateTimeUtc;

                foreach (var token in patientTokens)
                {
                    await _appointmentReminderService.SendNotificationAsync(
                        token,
                        utcTime,
                        "Doctor",
                        "Clinic"
                    );
                }

                foreach (var token in doctorTokens)
                {
                    await _appointmentReminderService.SendNotificationAsync(
                        token,
                        utcTime,
                        "Doctor",
                        "Clinic"
                    );
                }
                await _notificationService.SendAsync(patientUserId, "1 hour left to your appointment", "Reminder", null, appt.SlotId);

                await _notificationService.SendAsync(doctorUserId, "1 hour left to your appointment", "Reminder", null, appt.SlotId);

                appt.IsReminderSent = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"Error sending reminder: {ex.Message}");

            }
        }
    }
}