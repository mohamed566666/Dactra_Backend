using Dactra.DTOs.QuestionDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IQuestionAnswerService _answerService;
        private readonly IQuestionInterestService _interestService;
        private readonly IQuestionSaveService _saveService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public QuestionsController(
            IQuestionService questionService,
            IQuestionAnswerService answerService,
            IQuestionInterestService interestService,
            IQuestionSaveService saveService,
            IPatientService patientService,
            IDoctorService doctorService)
        {
            _questionService = questionService;
            _answerService = answerService;
            _interestService = interestService;
            _saveService = saveService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        // ── Questions ─────────────────────────────────────────────────────────

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<QuestionFeedResponseDto>> GetAll(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(await _questionService.GetAllAsync(page, pageSize, userId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<QuestionResponseDto>> GetById(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(await _questionService.GetByIdAsync(id, userId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("patient/{patientId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<QuestionResponseDto>>> GetByPatient(
            int patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(await _questionService.GetByPatientIdAsync(patientId, page, pageSize));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>Patient only: Get my questions.</summary>
        [HttpGet("my-questions")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<PagedResultDto<QuestionResponseDto>>> GetMyQuestions(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var patientId = await GetPatientId();
                return Ok(await _questionService.GetMyQuestionsAsync(patientId, page, pageSize));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>Filter: Interested | Saved | Answered</summary>
        [HttpGet("filterOn")]
        public async Task<ActionResult<PagedResultDto<QuestionResponseDto>>> GetMyFiltered(
            [FromQuery] QuestionFilterDto filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _questionService.GetMyFilteredQuestionsAsync(filter, userId, page, pageSize));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>Patient only: Create a question.</summary>
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<QuestionResponseDto>> Create([FromBody] CreateQuestionDto dto)
        {
            try
            {
                var patientId = await GetPatientId();
                var result = await _questionService.CreateAsync(dto, patientId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>Patient only: Update own question.</summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<QuestionResponseDto>> Update(int id, [FromBody] UpdateQuestionDto dto)
        {
            try
            {
                var patientId = await GetPatientId();
                return Ok(await _questionService.UpdateAsync(id, dto, patientId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>Patient only: Soft-delete own question.</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var patientId = await GetPatientId();
                await _questionService.DeleteAsync(id, patientId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Answers ───────────────────────────────────────────────────────────

        [HttpGet("{questionId:int}/answers")]
        [AllowAnonymous]
        public async Task<ActionResult<List<AnswerResponseDto>>> GetAnswers(int questionId)
        {
            try
            {
                return Ok(await _answerService.GetByQuestionIdAsync(questionId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>Doctor only: Answer a question.</summary>
        [HttpPost("{questionId:int}/answers")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<AnswerResponseDto>> AddAnswer(int questionId, [FromBody] CreateAnswerDto dto)
        {
            try
            {
                var doctorId = await GetDoctorId();
                return Ok(await _answerService.CreateAsync(questionId, dto, doctorId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>Doctor only: Update own answer.</summary>
        [HttpPut("answers/{answerId:int}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<AnswerResponseDto>> UpdateAnswer(int answerId, [FromBody] UpdateAnswerDto dto)
        {
            try
            {
                var doctorId = await GetDoctorId();
                return Ok(await _answerService.UpdateAsync(answerId, dto, doctorId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>Doctor only: Soft-delete own answer.</summary>
        [HttpDelete("answers/{answerId:int}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteAnswer(int answerId)
        {
            try
            {
                var doctorId = await GetDoctorId();
                await _answerService.DeleteAsync(answerId, doctorId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Interest ──────────────────────────────────────────────────────────

        /// <summary>Toggle interest on a question.</summary>
        [HttpPost("{questionId:int}/interest")]
        [AllowAnonymous]
        public async Task<ActionResult<QuestionInterestResponseDto>> ToggleInterest(int questionId)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _interestService.ToggleInterestAsync(questionId, userId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Save ──────────────────────────────────────────────────────────────

        /// <summary>Toggle save on a question.</summary>
        [HttpPost("{questionId:int}/save")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> ToggleSave(int questionId)
        {
            try
            {
                var userId = GetUserId();
                var isSaved = await _saveService.ToggleSaveAsync(questionId, userId);
                return Ok(new { questionId, isSaved });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("saved")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<SavedQuestionResponseDto>>> GetSaved(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _saveService.GetSavedQuestionsAsync(userId, page, pageSize));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("tag/{tagId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<QuestionResponseDto>>> GetByTag(
    int tagId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(await _questionService.GetByTagAsync(tagId, page, pageSize));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");
            return userId;
        }

        private async Task<int> GetPatientId()
        {
            var userId = GetUserId();
            var profile = await _patientService.GetProfileByUserID(userId);
            if (profile == null)
                throw new UnauthorizedAccessException("Patient profile not found.");
            return profile.Id;
        }

        private async Task<int> GetDoctorId()
        {
            var userId = GetUserId();
            var profile = await _doctorService.GetProfileByUserIdAsync(userId);
            if (profile == null)
                throw new UnauthorizedAccessException("Doctor profile not found.");
            return profile.Id;
        }
    }
}
