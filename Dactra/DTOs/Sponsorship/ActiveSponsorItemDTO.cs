namespace Dactra.DTOs.Sponsorship
{
    public class ActiveSponsorItemDTO
    {
        public int SponsorshipId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorPhoneNumber { get; set; } = string.Empty;
        public string? imageUrl { get; set; }
        public string DoctorSpeciality { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public string Description { get; set; } = string.Empty;
        public int PatientsSentCount { get; set; }
        public DateTime OfferDate { get; set; }
    }
}
