using Dactra.DTOs.TagDTOs;

namespace Dactra.DTOs.QuestionDTOs
{
    public class QuestionResponseDto
    {
        public int Id { get; set; }
        public string email { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public PatientSummaryDto Patient { get; set; } = null!;
        public int AnswersCount { get; set; }
        public int InterestsCount { get; set; }
        public int SavesCount { get; set; }
        public bool IsInterestedByCurrentUser { get; set; }
        public bool IsSavedByCurrentUser { get; set; }
        public List<TagDto> Tags { get; set; } = new();
        public QuestionStatsDto? UserStats { get; set; }
    }
}
