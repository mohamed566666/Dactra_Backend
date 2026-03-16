namespace Dactra.DTOs.QuestionDTOs
{
    public class SavedQuestionResponseDto
    {
        public int Id { get; set; }
        public QuestionSummaryDto Question { get; set; } = null!;
        public DateTime SavedAt { get; set; }
    }
}
