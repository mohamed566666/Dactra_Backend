using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface IDoctorProfileRepository
    {
        Task<DoctorProfile> GetByIdAsync(int id);
        Task<IEnumerable<DoctorProfile>> GetAllAsync();
        Task<DoctorProfile> GetByUserIdAsync(string userId);
        Task AddAsync(DoctorProfile profile);
        Task UpdateAsync(DoctorProfile profile);
        Task DeleteAsync(DoctorProfile profile);
    }
}
