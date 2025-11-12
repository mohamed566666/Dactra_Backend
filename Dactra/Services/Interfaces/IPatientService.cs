using Dactra.DTOs.ProfilesDTO;
using Dactra.Models;

namespace Dactra.Services.Interfaces
{
    public interface IPatientService
    {
        public Task CompleteRegistrationAsync(PatientCompleteDTO doctorComplateDTO);
        public Task<IEnumerable<PatientProfile>> GetAllProfileAsync();
    }
}
