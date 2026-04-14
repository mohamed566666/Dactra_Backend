using Dactra.DTOs.FileUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(IFileService fileService, ILogger<FileUploadController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// رفع ملف إلى Cloudinary
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        public async Task<IActionResult> UploadFile([FromForm] FileUploadRequestDTO request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { message = "الملف مطلوب" });

            var maxMB = 10; // أو اربطه بـ CloudinarySettings.MaxFileSizeMB
            var result = await _fileService.UploadAsync(request.File, request.Category ?? "general", maxMB);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// حذف ملف من Cloudinary
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile([FromQuery] string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return BadRequest(new { message = "معرف الملف (PublicId) مطلوب" });

            var success = await _fileService.DeleteAsync(publicId);
            return success
                ? Ok(new { message = "تم حذف الملف بنجاح" })
                : NotFound(new { message = "فشل في الحذف أو الملف غير موجود" });
        }
    }
}
