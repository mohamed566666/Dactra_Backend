using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface IDoctorProfileRepository : IGenericRepository<DoctorProfile>
    {
        Task<DoctorProfile?> GetByUserIdAsync(string userId);
        Task<DoctorProfile?> GetByUserEmail(string email);
    }
}
