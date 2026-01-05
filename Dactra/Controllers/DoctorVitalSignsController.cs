namespace Dactra.Controllers
{
    [Route("api/Doctor/vitals")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class DoctorVitalSignsController : ControllerBase
    {
        private readonly IVitalSignService _service;
        public DoctorVitalSignsController(IVitalSignService service)
        {
            _service = service;
        }

        [HttpGet("{patientId}")]
        public async Task<IActionResult> GetPatientVitals(int patientId)
        {
            var vitals = await _service.GetByPatientIdAsync(patientId);
            if (vitals == null || !vitals.Any())
                return NotFound("No vital signs found for this patient.");
            return Ok(vitals);
        }
    }
}
