namespace Dactra.DTOs.Sponsorship
{
    public class SponsorshipResponseDTO
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int MedicalTestProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public MedicalTestProviderType ProviderType { get; set; }
        public string OfferContent { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public SponsorshipStatus Status { get; set; }
        public bool IsCounterOffer { get; set; }
        public int? ParentOfferId { get; set; }
        public DateTime RequestedAtUtc { get; set; }
        public DateTime? RespondedAtUtc { get; set; }
        public List<SponsorshipResponseDTO> CounterOffers { get; set; } = new();
    }
}
