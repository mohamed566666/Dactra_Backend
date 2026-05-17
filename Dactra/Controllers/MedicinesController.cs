using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public MedicinesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchMedicines(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<string>());

            var medicines = await _context.Medicines
                .Where(m => EF.Functions.Like(m.Name, $"{query}%"))
                .Select(m => m.Name)
                .Distinct()
                .Take(10)
                .ToListAsync();

            return Ok(medicines);
        }
    }
}
