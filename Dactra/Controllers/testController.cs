using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IReminderService _reminderService;
        public testController(ApplicationDbContext context, IReminderService reminderService)
        {
            _context = context;
            _reminderService = reminderService;
         }

        [HttpGet]
        public async Task<IActionResult> Get(int appointmentId)
        {
            await _reminderService.SendReminder(appointmentId);

            return Ok();
        }
    }
}
