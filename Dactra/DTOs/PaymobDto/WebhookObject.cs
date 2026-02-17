namespace Dactra.DTOs.PaymobDto
{
    public class WebhookObject
    {
        public bool Success { get; set; }
        public WebhookOrder Order { get; set; } = new WebhookOrder();
    }
}
