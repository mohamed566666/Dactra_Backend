using Dactra.DTOs.ProfilesDTOs.DoctorDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IDoctorQualificationService
    {
        Task<IEnumerable<DoctorQualificationResponseDTO>> GetAllAsync(int doctorId);
        Task<IEnumerable<DoctorQualificationResponseDTO>> GetByUserIdAsync(string userId);
        Task<bool> CreateAsync(int doctorId, DoctorQualificationDTO dto);
        Task<bool> UpdateAsync(int qualificationId, DoctorQualificationDTO dto);
        Task<bool> DeleteAsync(int qualificationId);
        Task CreateByUserIdAsync(string userId, DoctorQualificationDTO dto);
        Task<bool> UpdateByUserIdAsync(string userId, int qualificationId, DoctorQualificationDTO dto);
        Task<bool> DeleteByUserIdAsync(string userId, int qualificationId);
    }
}
