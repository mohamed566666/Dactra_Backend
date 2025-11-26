using Dactra.DTOs.MajorDTOs;
using Dactra.Models;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorsController : ControllerBase
    {
        private readonly IMajorsService _majorsService;
        public MajorsController(IMajorsService majorsService)
        {
            _majorsService = majorsService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] MajorBasicsDTO major)
        {
            await _majorsService.CreateMajorAsync(major);
            return Ok("Major created successfully");
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var majors = await _majorsService.GetAllMajorsAsync();
            return Ok(majors);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var major = await _majorsService.GetMajorByIdAsync(id);
            if (major == null)
            {
                return NotFound("Major Not Found");
            }
            return Ok(major);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MajorBasicsDTO major)
        {
            await _majorsService.UpdateMajorAsync(id, major);
            return Ok("Major Updated successfully");
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateIcon(int id, string iconUrl)
        {
            await _majorsService.UpdateMajorIconAsync(id, iconUrl);
            return Ok("Major icon updated successfully");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _majorsService.DeleteMajorAsync(id);
            return Ok("Major deleted successfully");
        }
    }
}
