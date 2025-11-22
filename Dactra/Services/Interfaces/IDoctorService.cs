using Dactra.DTOs.ProfilesDTO;
using Dactra.DTOs.ProfilesDTOs;
using Dactra.Models;
using Dactra.Repositories.Interfaces;

namespace Dactra.Services.Interfaces
{
    public interface IDoctorService
    {
        public Task CompleteRegistrationAsync(DoctorCompleteDTO doctorComplateDTO);
        public Task<IEnumerable<DoctorProfileResponseDTO>> GetAllProfileAsync();
        public Task DeleteDoctorProfileAsync(int doctorProfileId);
    }
}
