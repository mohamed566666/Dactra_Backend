using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoCallController : ControllerBase
    {
        private readonly IVideoCallService _videoCallService;

        public VideoCallController(IVideoCallService videoCallService)
        {
            _videoCallService = videoCallService;
        }

        [HttpPost("join/{appointmentId:int}")]
        public async Task<IActionResult> JoinRoom(int appointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Not Valid Token");

            var result = await _videoCallService.JoinRoomAsync(appointmentId, userId);
            return Ok(result);
        }

        [HttpGet("status/{appointmentId:int}")]
        public async Task<IActionResult> GetRoomStatus(int appointmentId)
        {
            var result = await _videoCallService.GetRoomStatusAsync(appointmentId);
            return Ok(result);
        }

        [HttpPost("end/{appointmentId:int}")]
        public async Task<IActionResult> EndCall(int appointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException();

            await _videoCallService.EndCallAsync(appointmentId, userId);
            return Ok(new { message = "Call ended successfully" });
        }
    }
}
