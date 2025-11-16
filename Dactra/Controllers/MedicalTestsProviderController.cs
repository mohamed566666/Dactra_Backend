using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalTestsProviderController : ControllerBase
    {
        private readonly IMedicalTestsProviderService _medicalTestsProviderService;
        public MedicalTestsProviderController(IMedicalTestsProviderService medicalTestsProviderService)
        {
            _medicalTestsProviderService = medicalTestsProviderService;
        }

        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(DTOs.ProfilesDTO.MedicalTestProviderDTO medicalTestProviderDTO)
        {
            await _medicalTestsProviderService.CompleteRegistrationAsync(medicalTestProviderDTO);
            return Ok();
        }

        [HttpGet("GetAllProfiles")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetAllProfilesAsync();
            return Ok(MedicalTestProviderProfiles);
        }

        [HttpDelete("DeleteMedicalTestProvider/{medicalTestProviderId}")]
        public async Task<IActionResult> DeleteMedicalTestProvider(int medicalTestProviderId)
        {
            await _medicalTestsProviderService.DeleteMedicalTestProviderProfileAsync(medicalTestProviderId);
            return Ok("Profile Deleted Succesfully");
        }
    }
}
