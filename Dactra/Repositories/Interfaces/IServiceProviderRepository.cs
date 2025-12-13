namespace Dactra.Repositories.Interfaces
{
    public interface IServiceProviderRepository
    {
        Task<ServiceProviderProfile?> GetByIdAsync(int id);
        Task<ServiceProviderProfile?> GetByUserIdAsync(string userId);
        Task SaveChangesAsync();
    }
}
