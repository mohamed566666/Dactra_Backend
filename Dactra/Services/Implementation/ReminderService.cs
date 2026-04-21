
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


        public async Task SendReminder(int slotID)
        {
            var appt = _context.PatientAppointments
                .Include(a => a.Slot)
                .FirstOrDefault(a => a.SlotId == slotID);

            if (appt == null || appt.IsReminderSent)
            {
                return ;
            }


         
            await _notificationService.SendAsync(appt.PatientId.ToString(), "1 hour left to your appointment", "Reminder",null,slotID);

            await _notificationService.SendAsync(appt.Slot.DoctorId.ToString(), "1 hour left to your appointment", "Reminder", null, slotID);

            appt.IsReminderSent = true;
            await _context.SaveChangesAsync();

        }
    }
}
