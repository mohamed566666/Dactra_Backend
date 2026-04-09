namespace Dactra.Services.Interfaces
{
    public interface INotificationService
    {
        public  Task Send(string userId, string msg);
    }
}
