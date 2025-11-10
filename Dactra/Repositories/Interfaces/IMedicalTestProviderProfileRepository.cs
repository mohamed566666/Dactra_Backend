using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface IMedicalTestProviderProfileRepository
    {
        Task<MedicalTestProviderProfile> GetByIdAsync(int id);
        Task<IEnumerable<MedicalTestProviderProfile>> GetAllAsync();
        Task<MedicalTestProviderProfile> GetByUserIdAsync(string userId);
        Task AddAsync(MedicalTestProviderProfile profile);
        Task UpdateAsync(MedicalTestProviderProfile profile);
        Task DeleteAsync(MedicalTestProviderProfile profile);
    }
}
