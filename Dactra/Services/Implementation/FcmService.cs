namespace Dactra.Services.Implementation
{
    public class FcmService : IFcmService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ILogger<FcmService> _logger;

        public FcmService(HttpClient http, IConfiguration config, ILogger<FcmService> logger)
        {
            _http = http;
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendNotificationAsync(
            string fcmToken,
            string title,
            string body,
            Dictionary<string, string>? data = null)
        {
            var serverKey = _config["Firebase:ServerKey"]
                ?? throw new InvalidOperationException("Firebase:ServerKey is missing");

            var payload = new
            {
                to = fcmToken,
                notification = new { title, body },
                data = data ?? new Dictionary<string, string>(),
                android = new
                {
                    priority = "high",
                    notification = new
                    {
                        channel_id = "medicine_reminders",
                        sound = "default",
                        click_action = "FLUTTER_NOTIFICATION_CLICK"
                    }
                },
                apns = new
                {
                    headers = new { apns_priority = "10" },
                    payload = new { aps = new { sound = "default", badge = 1 } }
                },
                webpush = new
                {
                    headers = new { Urgency = "high" },
                    notification = new
                    {
                        title,
                        body,
                        icon = "/logo192.png",
                        badge = "/badge.png",
                        tag = "medicine-reminder",
                        requireInteraction = true,
                        actions = new[]
                        {
                        new { action = "taken", title = "✅ أخدت العلاج" },
                        new { action = "snooze", title = "⏰ ذكرني بعد 30 دقيقة" }
                    }
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.TryAddWithoutValidation("Authorization", $"key={serverKey}");

            var response = await _http.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("FCM Error {StatusCode}: {Body}", response.StatusCode, responseBody);
                return false;
            }

            _logger.LogInformation("FCM sent to token ending ...{Token}: {Body}",
                fcmToken[^8..], responseBody);
            return true;
        }

        public async Task<int> SendBulkNotificationsAsync(
            IEnumerable<string> fcmTokens,
            string title,
            string body,
            Dictionary<string, string>? data = null)
        {
            var tasks = fcmTokens.Select(token => SendNotificationAsync(token, title, body, data));
            var results = await Task.WhenAll(tasks);
            return results.Count(r => r);
        }
    }
}
