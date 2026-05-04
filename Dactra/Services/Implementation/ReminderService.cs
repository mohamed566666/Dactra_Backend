
namespace Dactra.Services.Implementation
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public ReminderService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }


        public async Task SendReminder(int appointmentId)
        {
            var appt = await _context.PatientAppointments
             .Include(a => a.Slot)
             .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appt == null || appt.IsReminderSent)
            {
                return;
            }

            var patientUserId = await _context.Patients.Where(p => p.Id == appt.PatientId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();
            var doctorUserId = await _context.Doctors.Where(d => d.Id == appt.Slot.DoctorId)
                .Select(d => d.UserId)
                .FirstOrDefaultAsync();

            await _notificationService.SendAsync(patientUserId, "1 hour left to your appointment", "Reminder",null,appt.SlotId);

            await _notificationService.SendAsync(doctorUserId, "1 hour left to your appointment", "Reminder", null, appt.SlotId);

            appt.IsReminderSent = true;
            await _context.SaveChangesAsync();

        }
    }
}
