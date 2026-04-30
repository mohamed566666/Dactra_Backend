

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
        [AllowAnonymous]
        public async Task<IActionResult> Refund(int appointmentid,string CancelledReason)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
                return Unauthorized("Invalid token");

            var patientId = await _context.PatientAppointments
                 .Where(a => a.Id == appointmentid)
                 .Select(a => a.PatientId)
                 .FirstOrDefaultAsync();
            if (patientId == null)
                return NotFound("Patient not found");

            bool result = await _service.CancelAppointmentAsync(appointmentid, patientId, CancelledReason, role);

            if (result)
                return Ok(new { success = true, message = "Appointment cancelled successfully." });
            else
                return BadRequest(new { success = false, message = "Cancellation failed." });


        }

        [HttpGet("doctor/{doctorId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorAppointments(int doctorId)
        {
            var appointments = await _service.GetDoctorAppointmentsAsync(doctorId);
            return Ok(appointments);
        }
    }
}
