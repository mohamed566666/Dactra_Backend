namespace Dactra.Models
{
    public class DoctorMedicalTestSponsor
    {
        [Key]
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public DoctorProfile Doctor { get; set; } = null!;

        public int MedicalTestProviderId { get; set; }
        public MedicalTestProviderProfile MedicalTestProvider { get; set; } = null!;

        public MedicalTestProviderType ProviderType { get; set; }

        public string OfferContent { get; set; } = string.Empty;

        public decimal DiscountPercentage { get; set; }

        public SponsorshipStatus Status { get; set; } = SponsorshipStatus.Pending;

        public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAtUtc { get; set; }

        public int? ParentOfferId { get; set; }
        public DoctorMedicalTestSponsor? ParentOffer { get; set; }

        public ICollection<DoctorMedicalTestSponsor> CounterOffers { get; set; }
            = new List<DoctorMedicalTestSponsor>();

        public bool IsCounterOffer { get; set; } = false;
    }
}
