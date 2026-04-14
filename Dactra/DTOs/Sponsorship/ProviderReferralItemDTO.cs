namespace Dactra.DTOs.Sponsorship
{
    public class ProviderReferralItemDTO
    {
        public int ReferralId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpeciality { get; set; } = string.Empty;
        public ReferralStatus Status { get; set; }
        public DateTime ReferredAtUtc { get; set; }
        public List<ReferralServiceItemDTO> Services { get; set; } = new();
        public decimal TotalPriceBeforeDiscount { get; set; }
        public decimal TotalPriceAfterDiscount { get; set; }
        public decimal TotalSaved { get; set; }
    }
}
