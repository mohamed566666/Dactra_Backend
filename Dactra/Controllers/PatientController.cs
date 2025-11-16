using Dactra.DTOs.ProfilesDTO;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }
        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(PatientCompleteDTO patientComplateDTO)
        {
            await _patientService.CompleteRegistrationAsync(patientComplateDTO);
            return Ok();
        }

        [HttpGet("GetAllProfiles")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var PatientProfiles = await _patientService.GetAllProfileAsync();
            return Ok(PatientProfiles);
        }

        [HttpDelete("DeletePatient/{patientId}")]
        public async Task<IActionResult> DeletePatient(int patientId)
        {
            await _patientService.DeletePatientProfileAsync(patientId);
            return Ok("Profile Deleted Succesfully");
        }
    }
}
