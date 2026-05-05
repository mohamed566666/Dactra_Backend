namespace Dactra.DTOs.FcmDtos
{
    public class SendFcmRequest
    {
        public string Token { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Body { get; set; }
        public Dictionary<string, string>? Data { get; set; }
    }
}
