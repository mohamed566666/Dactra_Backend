namespace Dactra.DTOs.QuestionDTOs
{
    public class AnswerResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public int QuestionId { get; set; }
        public int? ParentAnswerId { get; set; }
        public DoctorAnswerSummaryDto Doctor { get; set; } = null!;
        public List<AnswerResponseDto> Replies { get; set; } = new();
    }
}
