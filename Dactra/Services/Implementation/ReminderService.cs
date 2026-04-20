
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


            var notification = new
            {
                type = "Reminder",
                slotID = slotID,
                massage = "1 hour left to your appointment"
            };
            await _notificationService.Send(appt.PatientId.ToString(), notification);

            await _notificationService.Send(appt.Slot.DoctorId.ToString(),notification);

            appt.IsReminderSent = true;
            await _context.SaveChangesAsync();

        }
    }
}
