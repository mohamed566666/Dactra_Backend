namespace Dactra.DTOs.Sponsorship
{
    public class ReferralServiceItemDTO
    {
        public int ProviderOfferingId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceDescription { get; set; } = string.Empty;
        public decimal PriceBeforeDiscount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
