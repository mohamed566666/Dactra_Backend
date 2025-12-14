

namespace Dactra.Controllers
{
    [Authorize(Roles = "Patient")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;
        public AppointmentController(IAppointmentService service)
        {
            _service = service;
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
    }
}
