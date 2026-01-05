using Dactra.DTOs.VitalSignDTOs;

namespace Dactra.Controllers
{
    [Route("api/Patient/vitals")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientVitalSignsController : ControllerBase
    {
        private readonly IVitalSignService _service;
        public PatientVitalSignsController(IVitalSignService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] VitalSignCreateDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.AddVitalSignAsync(userId, dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetAllForPatientAsync(userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var deleted = await _service.DeleteVitalSignAsync(userId, id);
            if (!deleted) return NotFound();
            return Ok("Deleted successfully");
        }
    }
}
