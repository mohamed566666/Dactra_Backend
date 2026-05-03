namespace Dactra.DTOs.Sponsorship
{
    public class DoctorSponsorshipsResponseDTO
    {
        public int ActiveSponsorsCount { get; set; }
        public int PatientBenefitsCount { get; set; }
        public decimal AverageDiscountPercentage { get; set; }
        public List<SponsorshipResponseDTO> Sponsorships { get; set; } = new();
    }
}
