using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IPostService _postService;
        private readonly IUserRepository _userRepository;
        public NotificationController(INotificationService notificationService, IPostService postService, IUserRepository userRepository)
        {
            _notificationService = notificationService;
            _postService = postService;
            _userRepository = userRepository;
        }
        [HttpPost("send-to-me")]
        public async Task<IActionResult> Send([FromBody] NotificationMessageDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();
            if ( string.IsNullOrEmpty(dto.Message))
                return BadRequest("UserId and Message are required");


            await _notificationService.Send(currentUserId, dto.Message);
            return Ok("Notification sent");
        }
        [HttpPost("sent-to-user/{postId}")]
        public async Task<IActionResult> snedToUser(int postId, [FromBody] NotificationMessageDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();
            var post = await _postService.GetByIdAsync(postId);
            if (post == null)
                return NotFound($"Post with id {postId} not found");
            if (post.Doctor.Id.ToString() == currentUserId)
                return BadRequest("Cannot send notification to yourself");
            var username =await _userRepository.GetUserByIdAsync(currentUserId);

            var message = $"{username.UserName} {dto.Message}";

            await _notificationService.Send(post.Doctor.Id.ToString(),  message);

            return Ok("Notification sent to post owner");


        }
    }
}
