using System.Threading.Tasks;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IRatingService _ratingService;
        public readonly ApplicationDbContext _context;

        public PatientController(IPatientService patientService , IRatingService ratingService, ApplicationDbContext context)
        {
            _patientService = patientService;
            _ratingService = ratingService;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var PatientProfiles = await _patientService.GetAllProfileAsync();
            return Ok(PatientProfiles);
        }
        [Authorize]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            bool canView = await this.canView();
            if (!canView) {
                return StatusCode(StatusCodes.Status403Forbidden, "You don't have permission to view this profile");
            }
            var PatientProfile = await _patientService.GetProfileByIdAsync(Id);
            return PatientProfile == null ? NotFound("Patient Profile Not Found") : Ok(PatientProfile);
        }

        private async Task<bool> canView() {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return false;
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin") return true;
            if (role == "Patient")
            {
                var myProfile = _patientService.GetProfileByUserID(userId);
                if (myProfile != null && myProfile.Id == int.Parse(RouteData.Values["Id"].ToString()!)) return true;
                return false;
            }
            if (role == "Doctor")
            {
                var doctorId = await _context.Doctors
                    .Where(d => d.UserId == userId)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();
                var patientId = int.Parse(RouteData.Values["Id"].ToString()!);
                var inCare = await _context.PatientDoctorCares
                    .Where(d => d.IsActive && d.DoctorId == doctorId && d.PatientId == patientId && d.IsActive)
                    .AnyAsync();
                return inCare;
            }
            return false;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeletePatient(int Id)
        {
            try
            {
                await _patientService.DeletePatientProfileAsync(Id);
                return Ok("Profile Deleted Succesfully");
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(PatientCompleteDTO patientComplateDTO)
        {
            try
            {
                await _patientService.CompleteRegistrationAsync(patientComplateDTO);
                return Ok();
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetMe")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var PatientProfile = await _patientService.GetProfileByUserEmail(userEmail);
            return PatientProfile == null ? NotFound("Patient Profile Not Found") : Ok(PatientProfile);
        }

        [HttpGet("ByUserId/{Id}")]
        public async Task<IActionResult> GetById(string Id)
        {
            try
            {
                var profile = await _patientService.GetProfileByUserID(Id);
                return Ok(profile);
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(PatientUpdateDTO updateDTO)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Id == null)
            {
                return Unauthorized("User Not Logged In");
            }
            try
            {
                await _patientService.UpdateProfileAsync(Id, updateDTO);
                return Ok("Profile Updated Succesfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("{patientId}/allergies")]
        public async Task<IActionResult> GetAllergies(int patientId)
        {
            try
            {
                var allergies = await _patientService.GetAllergiesByPatientIdAsync(patientId);
                return Ok(allergies);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Patient with ID {patientId} not found");
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("{patientId}/chronic-diseases")]
        public async Task<IActionResult> GetChronicDiseases(int patientId)
        {
            try
            {
                var diseases = await _patientService.GetChronicDiseasesByPatientIdAsync(patientId);
                return Ok(diseases);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Patient with ID {patientId} not found");
            }
        }

        [Authorize]
        [HttpPut("allergies")]
        public async Task<IActionResult> UpdateAllergies([FromBody] PatientAllergiesUpdateDTO dto)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _patientService.UpdateAllergiesAsync(Id, dto.AllergyIds);
            return Ok("Allergies updated successfully");
        }

        [Authorize]
        [HttpPut("chronic-diseases")]
        public async Task<IActionResult> UpdateChronicDiseases([FromBody] PatientChronicDiseasesUpdateDTO dto)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _patientService.UpdateChronicDiseasesAsync(Id, dto.ChronicDiseaseIds);
            return Ok("Chronic Diseases updated successfully");
        }

        [Authorize]
        [HttpGet("allergies")]
        public async Task<IActionResult> GetMyAllergies()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allergies = await _patientService.GetMyAllergiesAsync(userId);
            return Ok(allergies);
        }

        [Authorize]
        [HttpGet("chronic-diseases")]
        public async Task<IActionResult> GetMyChronicDiseases()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var diseases = await _patientService.GetMyChronicDiseasesAsync(userId);
            return Ok(diseases);
        }
    }
}
