namespace Dactra.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(string userId, string message, string? title = null, string? type = null, int? entityId = null);

    }
}
