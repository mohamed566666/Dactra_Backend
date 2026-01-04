namespace Dactra.Models
{
    public class RateLimitSettings
    {
        public bool EnableRateLimiting { get; set; } = true;
        public int RequestLimit { get; set; } = 200;
        public int TimeWindowInSeconds { get; set; } = 60;
        public string BlockMessage { get; set; } = "Too many requests";
        public List<string> WhitelistedIPs { get; set; } = new List<string>();
    }
}
