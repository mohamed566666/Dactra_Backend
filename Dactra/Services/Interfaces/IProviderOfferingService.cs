namespace Dactra.Services.Interfaces
{
    public interface IProviderOfferingService
    {
        Task<IEnumerable<ProviderOffering>> GetAllAsync();
        Task<ProviderOffering?> GetByIdAsync(int id);
        Task<IEnumerable<ProviderOffering>> GetByProviderIdAsync(int providerId);
        Task<IEnumerable<ProviderOffering>> GetByServiceIdAsync(int serviceId);

        Task<ProviderOffering> CreateAsync(ProviderOfferingDto dto);
        Task<ProviderOffering?> UpdateAsync(int id, ProviderOfferingDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
