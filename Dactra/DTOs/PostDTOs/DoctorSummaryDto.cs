namespace Dactra.DTOs.PostDTOs
{
    public class DoctorSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string? Specialty { get; set; }
    }
}
