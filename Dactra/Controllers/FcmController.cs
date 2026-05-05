using Dactra.DTOs.FcmDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [ApiController]
    [Route("notifications")]
    public class FcmController : ControllerBase
    {
        private readonly IFcmService _fcmService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FcmController> _logger;

        public FcmController(
            IFcmService fcmService,
            ApplicationDbContext context,
            ILogger<FcmController> logger)
        {
            _fcmService = fcmService;
            _context = context;
            _logger = logger;
        }

        // ============================================================
        // POST /notifications/save-token
        // الفرونت بيبعت التوكن هنا بعد ما Firebase يديه ليه
        // ============================================================
        [HttpPost("save-token")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveToken([FromBody] SaveFcmTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest(new { message = "التوكن فارغ" });
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(patientId))
                return Unauthorized(new { message = "unauthorized" });

            // شوف لو التوكن موجود قبل كده
            var existing = await _context.NotificationSubscriptions
                .FirstOrDefaultAsync(s => s.FcmToken == request.Token);

            if (existing is null)
            {
                _context.NotificationSubscriptions.Add(new NotificationSubscription
                {
                    FcmToken = request.Token,
                    PatientId = patientId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                _logger.LogInformation("FCM Token saved for patient: {P}", patientId);
            }
            else
            {
                // لو موجود — تأكد إنه active
                existing.IsActive = true;
                existing.PatientId = patientId;
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "تم حفظ التوكن ✅" });
        }

        // ============================================================
        // POST /notifications/send
        // بتبعت notification لجهاز واحد بعينه
        // ============================================================
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendFcmRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest(new { message = "التوكن مطلوب" });

            var success = await _fcmService.SendNotificationAsync(
                fcmToken: request.Token,
                title: request.Title ?? "إشعار جديد",
                body: request.Body ?? "",
                data: request.Data
            );

            if (!success)
                return StatusCode(500, new { message = "فشل الإرسال ❌" });

            return Ok(new { message = "تم الإرسال ✅" });
        }

        // ============================================================
        // POST /notifications/broadcast
        // بتبعت لكل الأجهزة المسجّلة دفعة واحدة
        // ============================================================
        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastFcmRequest request)
        {
            var tokens = await _context.NotificationSubscriptions
                .Where(s => s.IsActive)
                .Select(s => s.FcmToken)
                .ToListAsync();

            if (!tokens.Any())
                return Ok(new { message = "مفيش أجهزة مسجّلة" });

            var successCount = await _fcmService.SendBulkNotificationsAsync(
                fcmTokens: tokens,
                title: request.Title ?? "إشعار جديد",
                body: request.Body ?? "",
                data: request.Data
            );

            return Ok(new
            {
                message = "تم الإرسال ✅",
                successCount,
                failureCount = tokens.Count - successCount
            });
        }
    }
}