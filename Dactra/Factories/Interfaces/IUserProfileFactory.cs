namespace Dactra.Factories.Interfaces
{
    public interface IUserProfileFactory
    {
        object CreateProfile(string role, string userId);
    }
}
