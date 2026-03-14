namespace Dactra.DTOs.PaymobDto
{
    public class PaymobWebhookDto
    {
        [JsonPropertyName("event")]
        public string? EventName { get; set; }

        [JsonPropertyName("obj")]
        public WebhookObject Obj { get; set; } = new WebhookObject();
    }
}
