namespace Dactra.Repositories.Interfaces
{
    public interface IProviderOfferingRepository : IGenericRepository<ProviderOffering>
    {
        Task<IEnumerable<ProviderOffering>> GetByProviderIdAsync(int providerId);
        Task<IEnumerable<ProviderOffering>> GetByServiceIdAsync(int serviceId);
    }
}
