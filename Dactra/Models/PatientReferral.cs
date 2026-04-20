namespace Dactra.Models
{
    public class PatientReferral
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient { get; set; } = null!;

        [Required]
        public int DoctorId { get; set; }
        [ForeignKey(nameof(DoctorId))]
        public DoctorProfile Doctor { get; set; } = null!;

        [Required]
        public int SponsorshipId { get; set; }
        [ForeignKey(nameof(SponsorshipId))]
        public DoctorMedicalTestSponsor Sponsorship { get; set; } = null!;

        public DateTime ReferredAtUtc { get; set; } = DateTime.UtcNow;

        public ReferralStatus Status { get; set; } = ReferralStatus.Pending;

        public DateTime? ReceivedAtUtc { get; set; }

        public ICollection<PatientReferralItem> ReferralServices { get; set; }
    = new List<PatientReferralItem>();
    }
}
