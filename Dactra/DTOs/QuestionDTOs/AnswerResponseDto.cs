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
        public AnswererInfoDto Answerer { get; set; } = null!;
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public int RepliesCount { get; set; }
    }
}
