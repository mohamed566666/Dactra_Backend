namespace Dactra.DTOs.PaymobDto
{
    public class WebhookOrder
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("transaction_Id")]
        public string? Transaction_Id { get; set; }

        [JsonPropertyName("amount_cents")]
        public int Amount_Cents { get; set; }
    }
}
