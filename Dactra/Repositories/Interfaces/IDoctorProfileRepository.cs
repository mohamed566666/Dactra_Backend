using Dactra.DTOs.DoctorSlotsDTOs;

namespace Dactra.Repositories.Interfaces
{
    public interface IDoctorProfileRepository : IGenericRepository<DoctorProfile>
    {
        Task<DoctorProfile?> GetByUserIdAsync(string userId);
        Task<DoctorProfile?> GetByUserEmail(string email);
        Task<(IEnumerable<DoctorProfile> doctors, int totalCount)> GetFilteredDoctorsAsync(DoctorFilterDTO filter);
        Task<IEnumerable<DoctorProfile>> GetApprovedDoctorsAsync();
        Task<IEnumerable<DoctorProfile>> GetdisApprovedDoctorsAsync();
        Task<WorkingHoursResponseDTO> GetWorkingHoursAsync(int doctorId);
        Task<bool> UpdateWorkingHoursAsync(int doctorId, WorkingHoursDTO workingHours);
    }
}
