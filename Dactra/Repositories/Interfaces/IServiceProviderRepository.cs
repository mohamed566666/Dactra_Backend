namespace Dactra.Repositories.Interfaces
{
    public interface IServiceProviderRepository
    {
        Task<ServiceProviderProfile?> GetByIdAsync(ServiceProviderType type, int id);
        Task SaveChangesAsync();
    }
}
