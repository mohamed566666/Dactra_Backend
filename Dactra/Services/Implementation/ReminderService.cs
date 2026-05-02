
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


         
            await _notificationService.SendAsync(appt.PatientId.ToString(), "1 hour left to your appointment", "Reminder",null,appt.SlotId);

            await _notificationService.SendAsync(appt.Slot.DoctorId.ToString(), "1 hour left to your appointment", "Reminder", null, appt.SlotId);

            appt.IsReminderSent = true;
            await _context.SaveChangesAsync();

        }
    }
}
