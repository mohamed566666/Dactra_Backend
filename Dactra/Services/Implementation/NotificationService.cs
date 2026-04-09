
namespace Dactra.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Send(string userId, string msg)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", msg);
        }
    }
}
