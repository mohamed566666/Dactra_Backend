using Dactra.DTOs.ProfilesDTOs.DoctorDTOs;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var DoctorProfiles = await _doctorService.GetAllProfileAsync();
            return Ok(DoctorProfiles);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var DoctorProfile = await _doctorService.GetProfileByIdAsync(Id);
            return DoctorProfile == null ? NotFound("Doctor Profile Not Found") : Ok(DoctorProfile);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteDoctor(int Id)
        {
            await _doctorService.DeleteDoctorProfileAsync(Id);
            return Ok("Profile Deleted Succesfully");
        }
        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(DoctorCompleteDTO doctorComplateDTO)
        {
            await _doctorService.CompleteRegistrationAsync(doctorComplateDTO);
            return Ok();
        }
        [HttpGet("GetByEmail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var DoctorProfile = await _doctorService.GetProfileByUserEmail(email);
            return DoctorProfile == null ? NotFound("Doctor Profile Not Found") : Ok(DoctorProfile);
        }
    }
}
