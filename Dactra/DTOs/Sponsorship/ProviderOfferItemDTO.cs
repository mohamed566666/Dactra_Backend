namespace Dactra.DTOs.Sponsorship
{
    public class ProviderOfferItemDTO
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpeciality { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime OfferDate { get; set; }
        public OfferFilterStatus Status { get; set; }
        public OriginalOfferSnapshotDTO? OriginalOffer { get; set; }
    }
}
