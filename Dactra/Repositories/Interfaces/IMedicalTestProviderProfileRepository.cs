using Dactra.Enums;
using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface IMedicalTestProviderProfileRepository : IGenericRepository<MedicalTestProviderProfile>
    {
        
        Task<MedicalTestProviderProfile?> GetByUserIdAsync(string userId);
        Task<IEnumerable<MedicalTestProviderProfile>> GetApprovedProfilesAsync(MedicalTestProviderType? type = null);
        Task<IEnumerable<MedicalTestProviderProfile>> GetProfilesByTypeAsync(MedicalTestProviderType type);
    }
}
