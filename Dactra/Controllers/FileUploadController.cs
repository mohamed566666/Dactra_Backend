using Dactra.DTOs.FileUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        //private readonly IFileService _fileService;
        //private readonly ILogger<FileUploadController> _logger;

        public FileUploadController()
        {
            //_fileService = fileService;
            //_logger = logger;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "ملف غير صالح" });

            return Ok("احا الشبشب ضاع");
        }
    }
}
