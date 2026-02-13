using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _service;

        public QuestionsController(IQuestionService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("AskQuestion")]
        public async Task<IActionResult> AskQuestion(CreateQuestionDto dto)
        {
            int patientId = User.GetPatientId(); 
            await _service.CreateQuestionAsync(patientId, dto);
            return Ok();
        }
        [Authorize(Roles = "Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, CreateQuestionDto dto)
        {
            int patientId = User.GetPatientId();
            await _service.UpdateQuestionAsync(id, patientId, dto);
            return Ok();
        }
        [Authorize(Roles = "Patient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            int patientId = User.GetPatientId();
            await _service.DeleteQuestionAsync(id, patientId);
            return Ok();
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("{id}/answer")]
        public async Task<IActionResult> Answer(int id, CreateAnswerDto dto)
        {
            int doctorId = User.GetDoctorId();
            await _service.AnswerQuestionAsync(id, doctorId, dto);
            return Ok();
        }
    }
}
