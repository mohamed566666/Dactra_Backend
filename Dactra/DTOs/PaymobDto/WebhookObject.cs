namespace Dactra.DTOs.PaymobDto
{
    public class WebhookObject
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("order")]
        public WebhookOrder Order { get; set; } = new WebhookOrder();
    }
}
