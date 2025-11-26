using Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs;
using Dactra.Enums;
using Dactra.Models;

namespace Dactra.Services.Interfaces
{
    public interface IMedicalTestsProviderService
    {
        public Task CompleteRegistrationAsync(MedicalTestProviderDTO medicalTestProviderDTO);
        public Task<IEnumerable<MedicalTestsProviderResponseDTO>> GetAllProfilesAsync();
        public Task DeleteMedicalTestProviderProfileAsync(int medicalTestProviderProfileId);
        public Task<MedicalTestsProviderResponseDTO> GetProfileByUserIdAsync(string userId);
        public Task<MedicalTestsProviderResponseDTO> GetProfileByIdAsync(int id);
        public Task UpdateProfileAsync(int id, MedicalTestsProviderUpdateDTO dto);
        public Task<IEnumerable<MedicalTestsProviderResponseDTO>> GetApprovedProfilesAsync(MedicalTestProviderType? type = null);
        public Task<IEnumerable<MedicalTestsProviderResponseDTO>> GetProfilesByTypeAsync(MedicalTestProviderType type);
        public Task ApproveProfileAsync(int id);
        public Task RejectProfileAsync(int id);
    }
}
