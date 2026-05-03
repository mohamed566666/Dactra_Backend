namespace Dactra.DTOs.Sponsorship
{
    public class DoctorMySponsorDTO
    {
        public int LabId { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string LabType { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
    }
}
