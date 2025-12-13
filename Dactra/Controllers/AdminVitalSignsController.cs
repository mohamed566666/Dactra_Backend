using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/Admin/vitals")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminVitalSignsController : ControllerBase
    {
        private readonly IVitalSignService _service;
        public AdminVitalSignsController(IVitalSignService service)
        {
            _service = service;
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetTypes()
        {
            var types = await _service.GetAllTypesAsync();
            return Ok(types);
        }

        [HttpPost("types")]
        public async Task<IActionResult> AddType([FromQuery] string name, [FromQuery] bool isComposite, [FromQuery] string? compositeFields)
        {
            var type = await _service.AddTypeAsync(name, isComposite, compositeFields);
            return Ok(type);
        }
    }
}
