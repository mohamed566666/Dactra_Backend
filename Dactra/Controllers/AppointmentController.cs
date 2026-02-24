

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
        [HttpPost ("Book")]
        public async Task<IActionResult> BookAppointment (int scheduleTableId)
        {
            int patientId = int.Parse(User.FindFirst("UserId")!.Value);
            try
            {
                var res = await _service.BookAppointmentAsync(patientId, scheduleTableId);
                return Ok(new { appointmentId = res });

            }
            catch(Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
        [HttpPost("paymob/webhook")]
        public async Task<IActionResult> PaymobWebhook([FromBody] PaymobWebhookDto data)
        {
            var orderId = data.Obj.Order.Id;
            var success = data.Obj.Success;

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymobOrderId == orderId);

            if (payment == null)
                return NotFound();

            if (success)
            {
                payment.Status = paymentStatus.Confirmed;

                var appointment = await _context.PatientAppointments
                    .FirstOrDefaultAsync(a => a.PaymentId == payment.Id);

                appointment.Status = AppointmentStatus.Confirmed;

                var slot = await _context.DoctorAvailabilitySlots
                    .FirstOrDefaultAsync(s => s.Id == appointment.SlotId);

                slot.IsBooked = true;

                await _context.SaveChangesAsync();
                slot.IsBooked = true;
                slot.IsReserved = false;
                slot.ReservedUntil = null;
                await _context.SaveChangesAsync();

            }
            else
            {
                payment.Status = paymentStatus.Failed;
            }

            return Ok();
        }

    }
}
