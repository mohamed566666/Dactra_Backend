using Dactra.DTOs.ProfilesDTO;
using Dactra.Models;
using Dactra.Repositories.Interfaces;

namespace Dactra.Services.Interfaces
{
    public interface IDoctorService
    {
        public Task CompleteRegistrationAsync(DoctorCompleteDTO doctorComplateDTO);
        public Task<IEnumerable<DoctorProfile>> GetAllProfileAsync();
    }
}
