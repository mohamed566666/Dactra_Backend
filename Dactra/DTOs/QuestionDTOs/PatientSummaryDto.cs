namespace Dactra.DTOs.QuestionDTOs
{
    public class PatientSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }
}
