namespace Dactra.Services.Interfaces
{
    public interface IDoctorService
    {
        Task CompleteRegistrationAsync(DoctorCompleteDTO doctorComplateDTO);
        Task DeleteDoctorProfileAsync(int doctorProfileId);
        Task<IEnumerable<DoctorProfileResponseDTO>> GetAllProfileAsync(int patientId = 0);
        Task<DoctorsResponseDTO> GetProfileByIdAsync(int doctorProfileId, int patientId = 0);
        Task<DoctorProfileResponseDTO> GetProfileByUserEmail(string email, int patientId = 0);
        Task<DoctorProfileResponseDTO> GetProfileByUserIdAsync(string userId);
        Task UpdateProfileAsync(string userId, DoctorUpdateDTO updatedProfile);
        Task<PaginatedDoctorsResponseDTO> GetFilteredDoctorsAsync(DoctorFilterDTO filter, int patientId = 0);
        Task<IEnumerable<DoctorProfileResponseDTO>> GetApprovedDoctorsAsync(int patientId = 0);
        Task<IEnumerable<DoctorProfileResponseDTO>> GetdisApprovedDoctorsAsync(int patientId = 0);
    }
}
