namespace Dactra.DTOs.Sponsorship
{
    public class ActiveSponsorItemDTO
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpeciality { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public string Description { get; set; } = string.Empty;
        public int PatientsSentCount { get; set; }
        public DateTime OfferDate { get; set; }
    }
}
