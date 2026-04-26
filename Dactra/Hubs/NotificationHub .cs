namespace Dactra.Hubs
{
    public class NotificationHub:Hub
    {
        public async Task SendNotification(string userId, string message)
        {
            Console.WriteLine($"Connected User: {Context.UserIdentifier}");
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
