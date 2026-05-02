

using Dactra.Enums;
using Dactra.Models;
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
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<AppointmentController> _logger;
        public AppointmentController(IAppointmentService service, ApplicationDbContext context, IPatientProfileRepository patientProfileRepository, IPaymentService paymentService, ILogger<AppointmentController> logger)
        {
            _service = service;
            _context = context;
            _patientProfileRepository = patientProfileRepository;
            _paymentService = paymentService;
            _logger = logger;
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
        public async Task<IActionResult> Refund(int appointmentid, string CancelledReason)
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
        [HttpGet("{appointmentId}/resume")]
        public async Task<IActionResult> ResumePayment(int appointmentId)
        {
            try
            {
               
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid token");

               
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (patient == null)
                    return Unauthorized("Patient not found");

                if (patient.User == null)
                    return BadRequest("User data missing");

                if (string.IsNullOrEmpty(patient.User.Email))
                    return BadRequest("User email missing");

                
                var appointment = await _context.PatientAppointments
                        .Include(a => a.Payment)
                        .AsSplitQuery()
                        .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null)
                    return NotFound("Appointment not found");

               
                if (appointment.PatientId != patient.Id)
                    return Forbid();

                var payment = appointment.Payment;

                if (payment == null)
                    return NotFound("Payment not found");

               
                if (payment.Status == paymentStatus.Confirmed)
                    return BadRequest("Payment already completed");

                if (payment.Status == paymentStatus.Cancelled)
                    return BadRequest("Payment was cancelled");

                if (payment.Amount <= 0)
                    return BadRequest("Invalid payment amount");

                _logger.LogInformation("Patient: {@patient}", patient);
                _logger.LogInformation("User: {@user}", patient?.User);
                _logger.LogInformation("Appointment: {@appointment}", appointment);
                _logger.LogInformation("Payment: {@payment}", appointment?.Payment);
                var paymentUrl = await _paymentService.GetPaymentUrl(
                    payment,
                    patient.User.UserName
                  );

                if (string.IsNullOrEmpty(paymentUrl))
                    return StatusCode(500, "Failed to generate payment URL");

               
                return Ok(new
                {
                    appointmentId = appointment.Id,
                    paymentId = payment.Id,
                    url = paymentUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Unexpected error occurred",
                    error = ex.Message
                });
            }
        }
    }

}