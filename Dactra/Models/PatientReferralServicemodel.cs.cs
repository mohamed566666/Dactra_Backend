namespace Dactra.Models
{
    public class PatientReferralItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientReferralId { get; set; }
        [ForeignKey(nameof(PatientReferralId))]
        public PatientReferral PatientReferral { get; set; } = null!;

        [Required]
        public int ProviderOfferingId { get; set; }
        [ForeignKey(nameof(ProviderOfferingId))]
        public ProviderOffering ProviderOffering { get; set; } = null!;
    }
}
