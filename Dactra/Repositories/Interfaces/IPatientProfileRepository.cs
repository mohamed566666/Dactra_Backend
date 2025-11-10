using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface IPatientProfileRepository
    {
        Task<PatientProfile> GetByIdAsync(int id);
        Task<IEnumerable<PatientProfile>> GetAllAsync();
        Task<PatientProfile> GetByUserIdAsync(string userId);
        Task AddAsync(PatientProfile profile);
        Task UpdateAsync(PatientProfile profile);
        Task DeleteAsync(PatientProfile profile);
    }
}
