namespace Dactra.Helpers
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string DefaultFolder { get; set; } = "Dactra";
        public int MaxFileSizeMB { get; set; } = 10;
    }
}
