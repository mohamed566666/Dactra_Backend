using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface IProviderOfferingRepository
    {
        Task<IEnumerable<ProviderOffering>> GetAllAsync();
        Task<ProviderOffering?> GetByIdAsync(int id);
        Task<IEnumerable<ProviderOffering>> GetByProviderIdAsync(int providerId);
        Task<IEnumerable<ProviderOffering>> GetByServiceIdAsync(int serviceId);

        Task AddAsync(ProviderOffering entity);
        void Update(ProviderOffering entity);
        void Delete(ProviderOffering entity);

        Task SaveAsync();
    }
}
