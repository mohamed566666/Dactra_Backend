using Dactra.DTOs;
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
        
        [HttpPost("AddMajor")]
        public async Task<IActionResult> CreateMajor([FromBody] MajorBasicsDTO major)
        {
            var createdMajor = await _majorsService.CreateMajorAsync(major);
            return CreatedAtAction(nameof(GetMajorById), new { id = createdMajor.Id }, createdMajor);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllMajors()
        {
            var majors = await _majorsService.GetAllMajorsAsync();
            var ret = majors.Select(m => new MajorsResponseDTO
            {
                Id = m.Id,
                Name = m.Name,
                IconPath = m.Iconpath,
                Description = m.Description
            }).ToList();
            return Ok(ret);
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetMajorById(int id)
        {
            var major = await _majorsService.GetMajorByIdAsync(id);
            if (major == null)
            {
                return NotFound();
            }
            var ret = new MajorsResponseDTO
            {
                Id = major.Id,
                Name = major.Name,
                IconPath = major.Iconpath,
                Description = major.Description
            };
            return Ok(major);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateMajor(int id, [FromBody] MajorBasicsDTO major)
        {
            await _majorsService.UpdateMajorAsync(id, major);
            return Ok("Major Updated successfully");
        }
        [HttpPatch("UpdateIcon/{id}")]
        public async Task<IActionResult> UpdateMajorIcon(int id, string iconUrl)
        {
            await _majorsService.UpdateMajorIconAsync(id, iconUrl);
            return Ok("Major icon updated successfully");
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteMajor(int id)
        {
            await _majorsService.DeleteMajorAsync(id);
            return Ok("Major deleted successfully");
        }
    }
}
