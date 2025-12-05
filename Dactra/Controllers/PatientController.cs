using AutoMapper;
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
            try
            {
                await _patientService.DeletePatientProfileAsync(Id);
                return Ok("Profile Deleted Succesfully");
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(PatientCompleteDTO patientComplateDTO)
        {
            try
            {
                await _patientService.CompleteRegistrationAsync(patientComplateDTO);
                return Ok();
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetMe")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var PatientProfile = await _patientService.GetProfileByUserEmail(userEmail);
            return PatientProfile == null ? NotFound("Patient Profile Not Found") : Ok(PatientProfile);
        }

        [HttpGet("ByUserId/{Id}")]
        public async Task<IActionResult> GetById(string Id)
        {
            try
            {
                var profile = await _patientService.GetProfileByUserID(Id);
                return Ok(profile);
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(PatientUpdateDTO updateDTO)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Id == null)
            {
                return Unauthorized("User Not Logged In");
            }
            try
            {
                await _patientService.UpdateProfileAsync(Id, updateDTO);
                return Ok("Profile Updated Succesfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
