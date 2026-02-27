using Dactra.DTOs.DoctorSlotsDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorSlotsController : ControllerBase
    {
        private readonly IDoctorSlotService _service;
        private readonly ApplicationDbContext _context;
        public DoctorSlotsController(IDoctorSlotService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        private async Task<int?> GetDoctorIdFromTokenAsync()
        {
            var userId = User.FindFirstValue("DoctorId")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return null;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null) return null;
            return doctor.Id;
        }

        [HttpGet("myworking-hours")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetWorkingHours()
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();

            try
            {
                var workingHours = await _service.GetWorkingHoursAsync(doctorId.Value);
                return Ok(workingHours);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error getting working hours for doctor {doctorId}" });
            }
        }

        [HttpGet("working-hours{doctorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWorkingHoursById(int doctorId)
        {
            try
            {
                var workingHours = await _service.GetWorkingHoursAsync(doctorId);
                return Ok(workingHours);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error getting working hours for doctor {doctorId}" });
            }
        }

        [HttpPut("working-details")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateWorkingHours([FromBody] WorkingHoursDTO workingHours)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();

            try
            {
                var result = await _service.UpdateWorkingHoursAsync(doctorId.Value, workingHours);
                return Ok(new
                {
                    message = "Working hours updated successfully",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error updating working hours for doctor {doctorId}" });
            }
        }


        [HttpPost("save-slots")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SaveSlots([FromBody] DoctorSlotsRequestDto dto)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();
            try {await _service.SaveSlotsAsync(doctorId.Value, dto.Slots);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(new { message = "Slots saved successfully" });
        }

        [HttpGet("all-slots")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetAllSlots()
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();

            var slots = await _service.GetAllSlotsAsync(doctorId.Value);
            return Ok(slots);
        }

        [HttpGet("all-slots{doctorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSlots(int doctorId)
        {
            var slots = await _service.GetAllFreeSlotsAsync(doctorId);
            return Ok(slots);
        }

        [HttpGet("range-slots-by-time")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetSlotsRange([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();

            var slots = await _service.GetSlotsAsync(doctorId.Value, fromUtc, toUtc);
            return Ok(slots);
        }

        [HttpGet("range-slots-by-time{doctorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSlotsRange(int doctorId , [FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc)
        {
            var slots = await _service.GetFreeSlotsAsync(doctorId, fromUtc, toUtc);
            return Ok(slots);
        }

        [HttpDelete("slots-on-day")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteSlotsByDay([FromQuery] DateTime dayUtc)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();

            await _service.DeleteSlotsByDayAsync(doctorId.Value, dayUtc);
            return Ok(new { message = "Slots deleted successfully" });
        }

        [HttpDelete("delete-slot{slotId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteSlot(int slotId)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();

            await _service.DeleteSlotAsync(doctorId.Value, slotId);
            return Ok(new { message = "Slot deleted successfully" });
        }
    }
}