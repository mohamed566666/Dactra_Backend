using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalReportController : ControllerBase
    {
        private readonly IMedicalReportService _medicalReportService;

        public MedicalReportController(IMedicalReportService medicalReportService)
        {
            _medicalReportService = medicalReportService;
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadMedicalReportDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User Not Logged In");

            try
            {
                var result = await _medicalReportService.UploadReportAsync(userId, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{reportId}")]
        public async Task<IActionResult> Delete(int reportId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User Not Logged In");

            try
            {
                await _medicalReportService.DeleteReportAsync(userId, reportId);
                return Ok("Medical Report Deleted Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("my-reports")]
        public async Task<IActionResult> GetMyReports()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User Not Logged In");

            try
            {
                var reports = await _medicalReportService.GetMyReportsAsync(userId);
                return Ok(reports);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpGet("patient/{patientProfileId}")]
        public async Task<IActionResult> GetByPatientId(int patientProfileId)
        {
            var reports = await _medicalReportService.GetReportsByPatientIdAsync(patientProfileId);
            return Ok(reports);
        }
    }
}
