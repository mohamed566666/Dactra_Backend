namespace Dactra.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProviderService _approvalService;

        public AdminController(IAdminService adminService, UserManager<ApplicationUser> userManager , IServiceProviderService approvalService)
        {
            _service = adminService;
            _userManager = userManager;
            _approvalService = approvalService;
        }
       
        [HttpPost("Add")]
        public async Task<IActionResult> AddAdmin(CreateAdminDto dto)
        {
            return Ok(await _service.AddAdmin(dto));
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAdmins()
        {
            return Ok(await _service.GetAdmins());
        }

     
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _service.GetById(id);
            if (user == null) return NotFound("Admin not found");
            return Ok(user);
        }

        [HttpGet("getByEmail")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _service.GetByEmail(email);
            if (user == null) return NotFound("Admin not found");
            return Ok(user);
        }
        [HttpDelete("DeleteAdmin/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteAppUser(id);

            if (!result)
                return NotFound(new { message = "user not found" });

            return Ok(new { message = "user Blocked/unblocked successfully" });
        }
        [HttpDelete("DeleteAppUser/{id}")]
        public async Task<IActionResult> DeleteAppUser(string id)
        {
            var result = await _service.DeleteAppUser(id);

            if (!result)
                return NotFound(new { message = "user not found" });

            return Ok(new { message = "user Blocked/unblocked successfully" });
        }
        [HttpDelete("DeleteQuestion{id}")]
        public async Task<IActionResult> DeleteQuestions(string id)
        {
            var result = await _service.DeleteQuestions(id);

            if (!result)
                return NotFound(new { message = "Question not found" });

            return Ok(new { message = "Question Blocked/unblocked successfully" });
        }
        [HttpDelete("DeletePost{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var result = await _service.DeletePosts(id);

            if (!result)
                return NotFound(new { message = "Post not found" });

            return Ok(new { message = "Post Blocked/unblocked successfully" });
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _service.GetSummary();
            return Ok(result);
        }
        [HttpGet("weekly-summary")]
        public async Task<IActionResult> GetWeeklySummary()
        {
            var result = await _service.GetWeeklyAppointmentsCount();
            return Ok(result);
        }
        [HttpGet("allPetientInfo")]
        public async Task<IActionResult> GetAllPatients(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _service.patientinfo(page, pageSize);
            return Ok(result);
        }
        [HttpGet("allquestionsInfo")]
        public async Task<IActionResult> GetAllquestions(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _service.questioninfo(page, pageSize);
            return Ok(result);
        }
        [HttpGet("allArticlesInfo")]
        public async Task<IActionResult> GetAllArticels(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var result = await _service.postinfo(page, pageSize);
            return Ok(result);
        }

        [HttpGet("allDoctorsInfo")]
        public async Task<IActionResult> GetDoctors(int page = 1, int pageSize = 10)
        {
            var result = await _service.GetDoctorsAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("allLabsInfo")]
        public async Task<IActionResult> GetLabs(int page = 1, int pageSize = 10)
        {
            var result = await _service.GetLabsAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("allScansInfo")]
        public async Task<IActionResult> GetScans(int page = 1, int pageSize = 10)
        {
            var result = await _service.GetScansAsync(page, pageSize);
            return Ok(result);
        }
        [HttpPut("approve-Provider")]
        public async Task<IActionResult> Approve([FromBody] ApprovalRequestDto dto)
        {
            var result = await _approvalService.ApproveAsync(dto.ProviderType, dto.ProviderId);
            if (!result)
                return NotFound("Service Provider not found");
            return Ok("Approved successfully");
        }

        [HttpPut("disapprove-Provider")]
        public async Task<IActionResult> DisApprove([FromBody] ApprovalRequestDto dto)
        {
            var result = await _approvalService.DisApproveAsync(dto.ProviderType, dto.ProviderId);
            if (!result)
                return NotFound("Service Provider not found");
            return Ok("Disapproved successfully");
        }
        //[HttpPost("makeAdmin/{id}")]
        //public async Task<IActionResult> MakeAdmin(string id)
        //{
        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null) return NotFound("User not found");

        //    await _service.AddToAdminRole(user);
        //    return Ok("User is now Admin");
        //}
    }
}
