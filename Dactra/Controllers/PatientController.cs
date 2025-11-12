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
        [HttpGet("GetAllProfile")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var PatientProfiles = await _patientService.GetAllProfileAsync();
            return Ok(PatientProfiles);
        }
        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(PatientCompleteDTO patientComplateDTO)
        {
            await _patientService.CompleteRegistrationAsync(patientComplateDTO);
            return Ok();
        }
    }
}
