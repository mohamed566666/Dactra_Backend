namespace Dactra.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimitSettings _settings;

        private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();

        public RateLimitingMiddleware(
            RequestDelegate next,
            IOptions<RateLimitSettings> settings)
        {
            _next = next;
            _settings = settings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_settings.EnableRateLimiting)
            {
                await _next(context);
                return;
            }

            if (context.Request.Path.StartsWithSegments("/appointmentHub"))
            {
                await _next(context);
                return;
            }

            var clientKey = GetClientKey(context);

            if (string.IsNullOrWhiteSpace(clientKey))
            {
                await _next(context);
                return;
            }

            if (_settings.WhitelistedIPs.Contains(clientKey))
            {
                await _next(context);
                return;
            }

            var clientInfo = _clients.GetOrAdd(clientKey, _ => new ClientRequestInfo());

            bool isRateLimited;

            lock (clientInfo)
            {
                var now = DateTime.UtcNow;
                var cutoff = now.AddSeconds(-_settings.TimeWindowInSeconds);

                while (clientInfo.RequestTimes.Count > 0 &&
                       clientInfo.RequestTimes.Peek() < cutoff)
                {
                    clientInfo.RequestTimes.Dequeue();
                }

                if (clientInfo.RequestTimes.Count >= _settings.RequestLimit)
                {
                    isRateLimited = true;
                }
                else
                {
                    clientInfo.RequestTimes.Enqueue(now);
                    clientInfo.LastRequestTime = now;
                    isRateLimited = false;
                }
            }

            if (isRateLimited)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";

                var response = System.Text.Json.JsonSerializer.Serialize(new
                {
                    message = _settings.BlockMessage,
                    retryAfterSeconds = _settings.TimeWindowInSeconds
                });

                await context.Response.WriteAsync(response);
                return;
            }

            if (_clients.Count > 1000)
            {
                CleanupOldClients();
            }

            await _next(context);
        }

        private string GetClientKey(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                return $"user:{context.User.Identity.Name}";
            }
            return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        private void CleanupOldClients()
        {
            var cutoff = DateTime.UtcNow.AddSeconds(-_settings.TimeWindowInSeconds * 2);

            foreach (var client in _clients)
            {
                if (client.Value.LastRequestTime < cutoff)
                {
                    _clients.TryRemove(client.Key, out _);
                }
            }
        }
    }

    public class ClientRequestInfo
    {
        public Queue<DateTime> RequestTimes { get; } = new();
        public DateTime LastRequestTime { get; set; } = DateTime.UtcNow;
    }
}
