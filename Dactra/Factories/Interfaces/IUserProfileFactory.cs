namespace Dactra.Factories.Interfaces
{
    public interface IUserProfileFactory
    {
        ProfileBase CreateProfile(string role, string userId);
    }
}
