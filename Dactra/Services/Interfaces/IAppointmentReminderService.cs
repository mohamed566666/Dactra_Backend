namespace Dactra.Services.Interfaces
{
    public interface IAppointmentReminderService
    {
        public  Task<bool> SendNotificationAsync(
              string fcmToken,
              DateTime appointmentTime,
              string doctorName,
              string clinicName,
              Dictionary<string, string>? extraData = null);
        public Task<int> SendBulkNotificationsAsync(
          IEnumerable<string> tokens,
          DateTime appointmentTime,
          string doctorName,
          string clinicName);
    }
}
