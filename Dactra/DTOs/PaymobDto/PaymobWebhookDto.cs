namespace Dactra.DTOs.PaymobDto
{
    public class PaymobWebhookDto
    {
        public string? Event { get; set; } 
        public WebhookObject Obj { get; set; } = new WebhookObject();
    }
}
