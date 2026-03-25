using Dactra.DTOs.TagDTOs;

namespace Dactra.DTOs.QuestionDTOs
{
    public class QuestionSummaryDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public int AnswersCount { get; set; }
        public int InterestsCount { get; set; }
        public List<TagDto> Tags { get; set; } = new();
    }
}
