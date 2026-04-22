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
            return doctor?.Id;
        }

        [HttpGet("myworking-hours")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetWorkingHours()
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();
            try
            {
                return Ok(await _service.GetWorkingHoursAsync(doctorId.Value));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error getting working hours" });
            }
        }

        [HttpGet("working-hours/{doctorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWorkingHoursById(int doctorId)
        {
            try
            {
                return Ok(await _service.GetWorkingHoursAsync(doctorId));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception) { return BadRequest(new { message = "Error getting working hours" }); }
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
                    message = $"{workingHours.Type} working hours updated successfully",
                    data = result
                });
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (Exception) { return BadRequest(new { message = "Error updating working hours" }); }
        }


        [HttpPost("save-slots/inperson")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SaveInPersonSlots([FromBody] DoctorSlotsRequestDto dto)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();
            try
            {
                await _service.SaveInPersonSlotsAsync(doctorId.Value, dto.Slots);
                return Ok(new { message = "InPerson slots saved successfully" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPost("save-slots/online")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SaveOnlineSlots([FromBody] DoctorSlotsRequestDto dto)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();
            try
            {
                await _service.SaveOnlineSlotsAsync(doctorId.Value, dto.Slots);
                return Ok(new { message = "Online slots saved successfully" });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpGet("all-slots/inperson")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetAllInPersonSlots()
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();
            return Ok(await _service.GetAllInPersonSlotsAsync(doctorId.Value));
        }

        [HttpGet("all-slots/online")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetAllOnlineSlots()
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();
            return Ok(await _service.GetAllOnlineSlotsAsync(doctorId.Value));
        }


        [HttpGet("all-slots/{doctorId}/inperson")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFreeInPersonSlots(int doctorId)
            => Ok(await _service.GetAllFreeInPersonSlotsAsync(doctorId));

        [HttpGet("all-slots/{doctorId}/online")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFreeOnlineSlots(int doctorId)
            => Ok(await _service.GetAllFreeOnlineSlotsAsync(doctorId));


        [HttpGet("range-slots/inperson")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetInPersonSlotsRange(
            [FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();
            return Ok(await _service.GetInPersonSlotsAsync(doctorId.Value, fromUtc, toUtc));
        }

        [HttpGet("range-slots/online")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetOnlineSlotsRange(
            [FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();
            return Ok(await _service.GetOnlineSlotsAsync(doctorId.Value, fromUtc, toUtc));
        }


        [HttpGet("range-slots/{doctorId}/inperson")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFreeInPersonSlotsRange(
            int doctorId, [FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc)
            => Ok(await _service.GetFreeInPersonSlotsAsync(doctorId, fromUtc, toUtc));

        [HttpGet("range-slots/{doctorId}/online")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFreeOnlineSlotsRange(
            int doctorId, [FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc)
            => Ok(await _service.GetFreeOnlineSlotsAsync(doctorId, fromUtc, toUtc));

        [HttpDelete("slots-on-day")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteSlotsByDay([FromQuery] DateTime dayUtc)
        {
            var doctorId = await GetDoctorIdFromTokenAsync();
            if (doctorId == null) return Unauthorized();
            await _service.DeleteSlotsByDayAsync(doctorId.Value, dayUtc);
            return Ok(new { message = "Slots deleted successfully" });
        }

        [HttpDelete("delete-slot/{slotId}")]
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