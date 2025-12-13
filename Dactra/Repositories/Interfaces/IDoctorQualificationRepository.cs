namespace Dactra.Repositories.Interfaces
{
    public interface IDoctorQualificationRepository : IGenericRepository<DoctorQualification>
    {
        Task<IEnumerable<DoctorQualification>> GetByDoctorIdAsync(int doctorId);
        Task<DoctorProfile?> GetByUserIdAsync(string userId);
    }
}
