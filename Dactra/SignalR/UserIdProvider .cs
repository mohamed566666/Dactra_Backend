namespace Dactra.SignalR
{
    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
           return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
