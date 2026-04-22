

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ApplicationDbContext _context;
        public NotificationService(IHubContext<NotificationHub> hubContext, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }
        public async Task SendAsync(string userId, string message, string? title = null, string? type = null, int? entityId = null)
        {
            var notification = new Notifications
            {
                UserId = userId,
                Title = title ?? "Notification",
                Message = message,
                Type = type,
                RelatedEntityId = entityId,
                CreatedAtUtc = DateTime.UtcNow
            };

            
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

           
            await _hubContext.Clients.User(userId)
                .SendAsync("ReceiveNotification", notification);
        }
    }
}
