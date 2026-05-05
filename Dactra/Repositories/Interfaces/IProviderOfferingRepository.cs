using Dactra.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dactra.Repositories.Interfaces
{
    public interface IProviderOfferingRepository : IGenericRepository<ProviderOffering>
    {
        Task<IEnumerable<ProviderOffering>> GetByProviderIdAsync(int providerId);
        Task<IEnumerable<ProviderOffering>> GetByServiceIdAsync(int serviceId);
        Task<ProviderOffering?> GetByProviderAndServiceAsync(int providerId, int testServiceId);
    }
}