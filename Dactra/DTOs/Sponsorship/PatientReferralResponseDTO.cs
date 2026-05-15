namespace Dactra.DTOs.Sponsorship
{
    public class PatientReferralResponseDTO
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int SponsorshipId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public ReferralStatus Status { get; set; }
        public DateTime ReferredAtUtc { get; set; }
        public DateTime? ReceivedAtUtc { get; set; }
        public List<ReferralServiceItemDTO> Services { get; set; } = new();
        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalAfterDiscount { get; set; }
        public decimal TotalSaved { get; set; }
        public string? PatientImageUrl { get; set; }
    }
}
