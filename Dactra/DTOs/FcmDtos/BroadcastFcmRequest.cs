namespace Dactra.DTOs.FcmDtos
{
    public class BroadcastFcmRequest
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        public Dictionary<string, string>? Data { get; set; }
    }
}
