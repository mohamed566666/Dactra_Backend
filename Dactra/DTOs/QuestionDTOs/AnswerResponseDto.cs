namespace Dactra.DTOs.QuestionDTOs
{
    public class AnswerResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int QuestionId { get; set; }
        public int? ParentAnswerId { get; set; }
        public DoctorAnswerSummaryDto Doctor { get; set; } = null!;
        public List<AnswerResponseDto> Replies { get; set; } = new();
    }
}
