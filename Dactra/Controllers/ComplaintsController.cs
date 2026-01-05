using Dactra.DTOs.ComplaintsDTOs;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintService _service;
        public ComplaintsController(IComplaintService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateComplaintDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _service.CreateAsync(userId, dto);
            return Ok("Complaint submitted successfully");
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> MyComplaints()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _service.GetMyComplaintsAsync(userId));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllComplaintsAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            return Ok(await _service.GetDetailsAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateComplaintStatusDTO dto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _service.UpdateStatusAsync(id, adminId, dto);
            return Ok("Complaint updated successfully");
        }
    }
}
