namespace Dactra.Services.Interfaces
{
    public interface IFcmService
    {
        Task<bool> SendNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null);
        Task<int> SendBulkNotificationsAsync(IEnumerable<string> fcmTokens, string title, string body, Dictionary<string, string>? data = null);
    }
}
