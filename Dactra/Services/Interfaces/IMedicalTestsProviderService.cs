using Dactra.DTOs.ProfilesDTO;
using Dactra.Models;

namespace Dactra.Services.Interfaces
{
    public interface IMedicalTestsProviderService
    {
        public Task CompleteRegistrationAsync(MedicalTestProviderDTO medicalTestProviderDTO);
        public Task<IEnumerable<MedicalTestProviderProfile>> GetAllProfilesAsync();
        public Task DeleteMedicalTestProviderProfileAsync(int medicalTestProviderProfileId);
    }
}
