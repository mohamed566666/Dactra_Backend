using Google.Apis.Auth.OAuth2;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Dactra.Services.Implementation
{
    public class FcmService : IFcmService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<FcmService> _logger;
        private readonly string _projectId;
        private readonly string _serviceAccountPath;

        public FcmService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<FcmService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;

            _projectId = _config["Firebase:project_id"]
                ?? throw new InvalidOperationException("Firebase:ProjectId is missing in appsettings.json");

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

            var token = await credential.UnderlyingCredential
                .GetAccessTokenForRequestAsync();

            return token;
        }

    
        public async Task<bool> SendNotificationAsync(
            string fcmToken,
            string title,
            string body,
            Dictionary<string, string>? data = null)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var client = _httpClientFactory.CreateClient();

                var url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

                var payload = new
                {
                    message = new
                    {
                        token = fcmToken,
                        notification = new { title, body },
                        data = data ?? new Dictionary<string, string>(),
                        android = new
                        {
                            priority = "high",
                            notification = new
                            {
                                channel_id = "medicine_reminders",
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
                                badge = "/badge.png",
                                tag = "medicine-reminder",
                                requireInteraction = true,
                                actions = new[]
                                {
                                    new { action = "taken",  title = "✅ أخدت العلاج" },
                                    new { action = "snooze", title = "⏰ ذكرني بعد 30 دقيقة" }
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
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("FCM Error {StatusCode}: {Body}", response.StatusCode, responseBody);
                    return false;
                }

                _logger.LogInformation("FCM sent to token ...{Token}: {Body}",
                    fcmToken[^8..], responseBody);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FCM Exception for token ...{Token}", fcmToken[^8..]);
                return false;
            }
        }

   
        public async Task<int> SendBulkNotificationsAsync(
            IEnumerable<string> fcmTokens,
            string title,
            string body,
            Dictionary<string, string>? data = null)
        {
            var tasks = fcmTokens.Select(token =>
                SendNotificationAsync(token, title, body, data));

            var results = await Task.WhenAll(tasks);
            return results.Count(r => r);
        }
    }
}