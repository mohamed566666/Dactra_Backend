using Dactra.DTOs.ProfilesDTOs.PatientDTOs;
using Dactra.Models;

namespace Dactra.Services.Interfaces
{
    public interface IPatientService
    {
        public Task CompleteRegistrationAsync(PatientCompleteDTO doctorComplateDTO);
        public Task<IEnumerable<PatientProfileResponseDTO>> GetAllProfileAsync();
        public Task DeletePatientProfileAsync(int patientProfileId);
        public Task<PatientProfileResponseDTO> GetProfileByUserID(string userId);
        public Task<PatientProfileResponseDTO> GetProfileByUserEmail(string email);
        public Task<PatientProfileResponseDTO> GetProfileByIdAsync(int patientProfileId);
        public Task UpdateProfileAsync(int patientProfileId, PatientProfile updatedProfile);
    }
}
