

using Dactra.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Controllers
{
    [Authorize(Roles = "Patient")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;
        private readonly ApplicationDbContext _context;
        public AppointmentController(IAppointmentService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }
        [HttpPost("Book")]
        public async Task<IActionResult> BookAppointment(int scheduleTableId)
        {

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid token");

                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == userId);
                var res = await _service.BookAppointmentAsync(patient.Id, scheduleTableId);
                return Ok(new { appointmentId = res });

            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
        [HttpPost("refund")]
        public async Task<IActionResult> Refund(int appointmentid)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token");
            var patient = await _context.Patients
                   .FirstOrDefaultAsync(p => p.UserId == userId);

            bool result = await _service.CancelAppointmentAsync(appointmentid,patient.Id);

            if (result)
                return Ok(new { success = true, message = "All payments refunded successfully." });
            else
                return BadRequest(new { success = false, message = "Some refunds failed. Check logs." });


        }
    }
}
