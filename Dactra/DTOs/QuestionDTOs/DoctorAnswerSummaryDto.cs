namespace Dactra.DTOs.QuestionDTOs
{
    public class DoctorAnswerSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
