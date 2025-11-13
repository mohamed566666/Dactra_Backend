using Dactra.DTOs.ProfilesDTO;
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
        [HttpGet("GetAllProfiles")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var DoctorProfiles = await _doctorService.GetAllProfileAsync();
            return Ok(DoctorProfiles);
        }
        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(DoctorCompleteDTO doctorComplateDTO)
        {
            await _doctorService.CompleteRegistrationAsync(doctorComplateDTO);
            return Ok();
        }
        [HttpDelete("DeleteDoctor/{doctorId}")]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            await _doctorService.DeleteDoctorProfileAsync(doctorId);
            return Ok();
        }
    }
}
