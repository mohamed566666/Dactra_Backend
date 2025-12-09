namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var DoctorProfiles = await _doctorService.GetAllProfileAsync();
            return Ok(DoctorProfiles);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var DoctorProfile = await _doctorService.GetProfileByIdAsync(Id);
            return DoctorProfile == null ? NotFound("Doctor Profile Not Found") : Ok(DoctorProfile);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteDoctor(int Id)
        {
            try
            {
                await _doctorService.DeleteDoctorProfileAsync(Id);
                return Ok("Profile Deleted Succesfully");
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(DoctorCompleteDTO doctorComplateDTO)
        {
            try
            {
                await _doctorService.CompleteRegistrationAsync(doctorComplateDTO);
                return Ok();
            }
            catch(Exception ex) {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("GetByEmail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                var DoctorProfile = await _doctorService.GetProfileByUserEmail(email);
                return DoctorProfile == null ? NotFound("Doctor Profile Not Found") : Ok(DoctorProfile);
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetMe")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found");
            }

            try
            {
                var profile = await _doctorService.GetProfileByUserIdAsync(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateDoctor(DoctorUpdateDTO doctorUpdateDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found");
            }
            try
            {
                await _doctorService.UpdateProfileAsync(userId , doctorUpdateDTO);
                return Ok("Profile Updated Successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<PaginatedDoctorsResponseDTO>> GetFilteredDoctors([FromQuery] DoctorFilterDTO filter)
        {
            try
            {
                var result = await _doctorService.GetFilteredDoctorsAsync(filter);

                if (result.TotalCount == 0)
                {
                    return Ok(new PaginatedDoctorsResponseDTO
                    {
                        Doctors = new List<DoctorsFilterResponseDTO>(),
                        CurrentPage = 1,
                        TotalPages = 0,
                        PageSize = filter.PageSize,
                        TotalCount = 0
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving doctors", error = ex.Message });
            }
        }
    }
}
