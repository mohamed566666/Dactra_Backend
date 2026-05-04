using Dactra.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Numerics;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IPostService _postService;
        private readonly IUserRepository _userRepository;
        private readonly IDoctorSlotService _doctorSlotService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorService _doctorService;
        private readonly IQuestionInterestService _questionInterestService;
        public NotificationController(
            INotificationService notificationService,
            IPostService postService, 
            IUserRepository userRepository,
            IDoctorSlotService doctorSlotService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IDoctorService doctorService,
            IQuestionInterestService questionInterestService
            )
        {
            _notificationService = notificationService;
            _postService = postService;
            _userRepository = userRepository;
            _doctorSlotService = doctorSlotService;
            _context = context;
            _userManager = userManager;
            _doctorService = doctorService;
            _questionInterestService = questionInterestService; 
        }
        [HttpPost("me")]
        public async Task<IActionResult> SendAsync([FromBody] NotificationMessageDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();
            if (string.IsNullOrEmpty(dto.Message))
                return BadRequest("UserId and Message are required");


            await _notificationService.SendAsync(currentUserId, dto.Message);
            return Ok("Notification sent");
        }
        [HttpPost("sent-to-doctor/{postId}")]
        public async Task<IActionResult> SendAsyncToDoctor(int postId, [FromBody] NotificationDTO dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();
            var post = await _postService.GetByIdAsync(postId);
            if (post == null)
                return NotFound($"Post with id {postId} not found");
            if (post.Doctor.Id.ToString() == currentUserId)
                return BadRequest("Cannot SendAsync notification to yourself");
            var username = await _userRepository.GetUserByIdAsync(currentUserId);

            var message = $"{username.UserName} {dto.Message}";

          
           var docId = await _context.Doctors.Where(d => d.Id == post.Doctor.Id).Select(d => d.UserId).FirstOrDefaultAsync();

            await _notificationService.SendAsync(docId.ToString(), message, dto.Title, dto.Type,postId);

            return Ok("Notification sent to post owner");


        }
        [HttpPost("cancel/{Slotid}")]
        public async Task<IActionResult> cancelAppointmentNotification(int Slotid, [FromBody] NotificationDTO dto)
        {

            var doctorId = await _doctorSlotService.GetDoctorIdBySlotId(Slotid);
            var patientId = await _context.PatientAppointments.Where(a => a.SlotId == Slotid)
                .Select(a => a.PatientId)
                .FirstOrDefaultAsync();
            if (patientId == 0)
                return NotFound("Patient not found");
            var docId= await _context.Doctors.Where(d => d.Id == doctorId).Select(d => d.UserId).FirstOrDefaultAsync();
            var patientUserId = await _context.Patients.Where(p => p.Id == patientId).Select(p => p.UserId).FirstOrDefaultAsync();

            await _notificationService.SendAsync(docId, dto.Message, dto.Title, dto.Type,Slotid);

            await _notificationService.SendAsync(patientUserId, dto.Message, dto.Title, dto.Type,Slotid);

            return Ok("Notification sent to patient and doctor");

        }
        [HttpPost("bookAppointmentNotification/{id}")]
        public async Task<IActionResult> bookAppointmentNotification(int id, [FromBody] NotificationDTO dto)
        {

            var doctorId = await _doctorSlotService.GetDoctorIdBySlotId(id);/////
            var docId = await _context.Doctors.Where(d => d.Id == doctorId).Select(d => d.UserId).FirstOrDefaultAsync();
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            await _notificationService.SendAsync(docId, dto.Message, dto.Title, dto.Type,id);
            await _notificationService.SendAsync(currentUserId.ToString(), dto.Message, dto.Title, dto.Type,id);
            return Ok("Notification sent to patient and doctor");
        }
        [HttpPost("new-user")]
        public async Task<IActionResult> NewUser([FromBody] NewUserNotificationDto dto)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                await _notificationService.SendAsync(admin.Id, $"new user ({dto.UserType}) registered ",null, "new_user");
            }

            return Ok("Notification sent to Admin");
        }
        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var notifications = await _context.Notifications
                .Where(n => n.UserId == currentUserId)
                .OrderByDescending(n => n.CreatedAtUtc)
                .ToListAsync();

            return Ok(notifications);
        }
        [HttpPost("read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == currentUserId);

            if (notification == null)
                return NotFound("Notification not found");

            notification.IsRead = true;

            await _context.SaveChangesAsync();

            return Ok(
                new {
                    message = $"Marked as read ",
                    data = notification
                }
             );
          
        }
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var count = await _context.Notifications
                .CountAsync(n => n.UserId == currentUserId && !n.IsRead);

            return Ok(new { count });
        }
        [HttpPost("interested-users/{questionId}")]
        public async Task<IActionResult> GetInterestedUsers(int questionId)
        {
            var userIds = await _questionInterestService.GetInterestedUsersIdAsync(questionId);
            foreach (var userId in userIds)
            {
                await _notificationService.SendAsync(userId,"Doctor answered your question","Question Interest","Question",questionId);
            }

            return Ok("notification sent to all interested users");
        }

        //[HttpGet("test-job")]
        //public IActionResult TestJob()
        //{
        //    BackgroundJob.Enqueue(() => Console.WriteLine("Hello Hangfire 🔥"));
        //    return Ok();
        //}
    }
}
