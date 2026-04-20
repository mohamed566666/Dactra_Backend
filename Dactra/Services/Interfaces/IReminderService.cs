namespace Dactra.Services.Interfaces
{
    public interface IReminderService
    {
        public  Task SendReminder(int appointmentId);
    }
}
