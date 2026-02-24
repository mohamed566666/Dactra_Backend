namespace Dactra.DTOs.PaymobDto
{
    public class WebhookOrder
    {

        public string Id { get; set; } = string.Empty;        
        public string? Transaction_Id { get; set; }             
        public int Amount_Cents { get; set; }
    }
}
