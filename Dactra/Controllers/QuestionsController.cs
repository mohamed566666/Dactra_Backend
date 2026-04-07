using Dactra.DTOs.QuestionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        private readonly IQuestionAnswerLikeService _answerLikeService;

        public QuestionsController(
            IQuestionService questionService,
            IQuestionAnswerService answerService,
            IQuestionInterestService interestService,
            IQuestionSaveService saveService,
            IPatientService patientService,
            IDoctorService doctorService,
            IQuestionAnswerLikeService answerLikeService)
        {
            _questionService = questionService;
            _answerService = answerService;
            _interestService = interestService;
            _saveService = saveService;
            _patientService = patientService;
            _doctorService = doctorService;
            _answerLikeService = answerLikeService;
        }

        private bool IsUserDoctor() => User.IsInRole("Doctor");

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");
            return userId;
        }

        private string? GetUserIdOrDefault() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

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

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<QuestionFeedResponseDto>> GetAll(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserIdOrDefault();
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
                var userId = GetUserIdOrDefault();
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

        [HttpGet("{questionId:int}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<AnswerResponseDto>>> GetComments(
            int questionId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var currentUserId = GetUserIdOrDefault();
                return Ok(await _answerService.GetTopLevelAnswersByQuestionIdAsync(
                    questionId, page, pageSize, currentUserId));
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

        [HttpGet("comments/{parentCommentId:int}/replies")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<AnswerResponseDto>>> GetReplies(
            int parentCommentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var currentUserId = GetUserIdOrDefault();
                return Ok(await _answerService.GetRepliesByParentAnswerIdAsync(
                    parentCommentId, page, pageSize, currentUserId));
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

        [HttpPost("{questionId:int}/comments")]
        public async Task<ActionResult<AnswerResponseDto>> AddComment(int questionId, [FromBody] CreateAnswerDto dto)
        {
            try
            {
                var userId = GetUserId();
                var isDoctor = IsUserDoctor();
                return Ok(await _answerService.CreateAsync(questionId, dto, userId, isDoctor));
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

        [HttpPost("comments/{commentId:int}/like")]
        public async Task<ActionResult<AnswerLikeResponseDto>> ToggleCommentLike(int commentId)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _answerLikeService.ToggleLikeAsync(commentId, userId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("comments/{commentId:int}")]
        public async Task<ActionResult<AnswerResponseDto>> UpdateComment(int commentId, [FromBody] UpdateAnswerDto dto)
        {
            try
            {
                var userId = GetUserId();
                var isDoctor = IsUserDoctor();
                return Ok(await _answerService.UpdateAsync(commentId, dto, userId, isDoctor));
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

        [HttpDelete("comments/{commentId:int}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                var userId = GetUserId();
                var isDoctor = IsUserDoctor();
                await _answerService.DeleteAsync(commentId, userId, isDoctor);
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

        [HttpPost("{questionId:int}/interest")]
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

        [HttpPost("{questionId:int}/save")]
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
    }
}