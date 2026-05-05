namespace Dactra.DTOs.FcmDtos
{
    public class SaveFcmTokenRequest
    {
        public string Token { get; set; } = string.Empty;
        public string? PatientId { get; set; }  
    }
}
