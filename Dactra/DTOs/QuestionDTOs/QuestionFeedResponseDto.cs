namespace Dactra.DTOs.QuestionDTOs
{
    public class QuestionFeedResponseDto
    {
        public PagedResultDto<QuestionResponseDto> Questions { get; set; } = null!;
        public UserQuestionStatsDto Stats { get; set; } = null!;
    }
}
