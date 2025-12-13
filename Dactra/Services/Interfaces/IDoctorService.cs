namespace Dactra.Services.Interfaces
{
    public interface IDoctorService
    {
        public Task CompleteRegistrationAsync(DoctorCompleteDTO doctorComplateDTO);
        public Task<IEnumerable<DoctorProfileResponseDTO>> GetAllProfileAsync();
        public Task<DoctorProfileResponseDTO> GetProfileByIdAsync(int doctorProfileId);
        public Task<DoctorProfileResponseDTO> GetProfileByUserEmail(string email);
        public Task<DoctorProfileResponseDTO> GetProfileByUserIdAsync(string userId);
        public Task DeleteDoctorProfileAsync(int doctorProfileId);
        public Task UpdateProfileAsync(string userId, DoctorUpdateDTO updatedProfile);
        public Task<PaginatedDoctorsResponseDTO> GetFilteredDoctorsAsync(DoctorFilterDTO filter);
        public Task<IEnumerable<DoctorProfileResponseDTO>> GetApprovedDoctorsAsync();
        public Task<IEnumerable<DoctorProfileResponseDTO>> GetdisApprovedDoctorsAsync();
    }
}
