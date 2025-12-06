using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface IPatientProfileRepository : IGenericRepository<PatientProfile>
    {
        Task<PatientProfile?> GetByUserIdAsync(string userId);
        Task<PatientProfile?> GetByUserEmail(string email);
    }
}
