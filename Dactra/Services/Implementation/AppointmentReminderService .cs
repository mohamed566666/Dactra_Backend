using Google.Apis.Auth.OAuth2;
using System.Net.Http.Headers;

namespace Dactra.Services.Implementation
{
    public class AppointmentReminderService: IAppointmentReminderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<AppointmentReminderService> _logger;
        private readonly string _projectId;
        private readonly string _serviceAccountPath;

        public AppointmentReminderService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<AppointmentReminderService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;

            _projectId = _config["Firebase:project_id"]
                ?? throw new InvalidOperationException("Firebase:ProjectId is missing");

            _serviceAccountPath = Path.Combine(
                AppContext.BaseDirectory,
                "firebase-adminsdk.json"
            );

            if (!File.Exists(_serviceAccountPath))
                throw new FileNotFoundException($"firebase-adminsdk.json مش موجود في: {_serviceAccountPath}");
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var credential = GoogleCredential
                .FromFile(_serviceAccountPath)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            return await credential.UnderlyingCredential
                .GetAccessTokenForRequestAsync();
        }

        public async Task<bool> SendNotificationAsync(
            string fcmToken,
            DateTime appointmentTime,
            string doctorName,
            string clinicName,
            Dictionary<string, string>? extraData = null)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var client = _httpClientFactory.CreateClient();

                var url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

                var title = "📅 تذكير بموعد الحجز";
                var body = $"عندك ميعاد مع د. {doctorName} في {clinicName} الساعة {appointmentTime:hh:mm tt}";

                var payload = new
                {
                    message = new
                    {
                        token = fcmToken,
                        notification = new { title, body },

                        data = new Dictionary<string, string>
                        {
                            { "type", "appointment_reminder" },
                            { "doctorName", doctorName },
                            { "clinicName", clinicName },
                            { "appointmentTime", appointmentTime.ToString("o") }
                        },

                        android = new
                        {
                            priority = "high",
                            notification = new
                            {
                                channel_id = "appointment_reminders",
                                sound = "default"
                            }
                        },

                        apns = new
                        {
                            headers = new { apns_priority = "10" },
                            payload = new
                            {
                                aps = new { sound = "default", badge = 1 }
                            }
                        },

                        webpush = new
                        {
                            headers = new { Urgency = "high" },
                            notification = new
                            {
                                title,
                                body,
                                icon = "/logo192.png",
                                tag = "appointment-reminder",
                                requireInteraction = true,
                                actions = new[]
                                {
                                    new { action = "open", title = "📍 عرض التفاصيل" },
                                    new { action = "snooze", title = "⏰ ذكرني بعد شوية" }
                                }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("FCM Error {StatusCode}: {Body}",
                        response.StatusCode, responseBody);
                    return false;
                }

                _logger.LogInformation("Appointment reminder sent to ...{Token}",
                    fcmToken[^8..]);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FCM Exception for token ...{Token}",
                    fcmToken[^8..]);
                return false;
            }
        }

        public async Task<int> SendBulkNotificationsAsync(
            IEnumerable<string> tokens,
            DateTime appointmentTime,
            string doctorName,
            string clinicName)
        {
            var tasks = tokens.Select(token =>
                SendNotificationAsync(token, appointmentTime, doctorName, clinicName));

            var results = await Task.WhenAll(tasks);
            return results.Count(r => r);
        }

 
    }
}
    

