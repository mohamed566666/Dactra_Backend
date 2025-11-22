using Dactra.DTOs.ProfilesDTO;
using Dactra.DTOs.ProfilesDTOs;
using Dactra.Enums;
using Dactra.Models;

namespace Dactra.Services.Interfaces
{
    public interface IMedicalTestsProviderService
    {
        public Task CompleteRegistrationAsync(MedicalTestProviderDTO medicalTestProviderDTO);
        public Task<IEnumerable<MedicalTestProviderProfile>> GetAllProfilesAsync();
        public Task DeleteMedicalTestProviderProfileAsync(int medicalTestProviderProfileId);
        public Task<MedicalTestProviderProfile> GetProfileByUserIdAsync(string userId);
        public Task<MedicalTestProviderProfile> GetProfileByIdAsync(int id);
        public Task UpdateProfileAsync(int id, MedicalTestsProviderUpdateDTO dto);
        public Task<IEnumerable<MedicalTestsProviderResponseDTP>> GetApprovedProfilesAsync(MedicalTestProviderType? type = null);
        public Task<IEnumerable<MedicalTestsProviderResponseDTP>> GetProfilesByTypeAsync(MedicalTestProviderType type);
        public Task ApproveProfileAsync(int id);
        public Task RejectProfileAsync(int id);
    }
}
