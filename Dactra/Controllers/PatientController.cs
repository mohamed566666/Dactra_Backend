using Dactra.DTOs.ProfilesDTOs.PatientDTOs;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var PatientProfiles = await _patientService.GetAllProfileAsync();
            return Ok(PatientProfiles);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var PatientProfile = await _patientService.GetProfileByIdAsync(Id);
            return PatientProfile == null ? NotFound("Patient Profile Not Found") : Ok(PatientProfile);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeletePatient(int Id)
        {
            await _patientService.DeletePatientProfileAsync(Id);
            return Ok("Profile Deleted Succesfully");
        }

        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(PatientCompleteDTO patientComplateDTO)
        {
            await _patientService.CompleteRegistrationAsync(patientComplateDTO);
            return Ok();
        }

        [HttpGet("GetMe")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var PatientProfile = await _patientService.GetProfileByUserEmail(userEmail);
            return PatientProfile == null ? NotFound("Patient Profile Not Found") : Ok(PatientProfile);
        }
    }
}
