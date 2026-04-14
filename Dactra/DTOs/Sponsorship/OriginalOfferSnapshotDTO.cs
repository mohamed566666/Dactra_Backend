namespace Dactra.DTOs.Sponsorship
{
    public class OriginalOfferSnapshotDTO
    {
        public int Id { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime OfferDate { get; set; }
    }
}
