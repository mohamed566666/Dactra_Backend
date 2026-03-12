namespace Dactra.DTOs.CommentDTOs
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
        public UserSummaryDto User { get; set; } = null!;
        public List<CommentResponseDto> Replies { get; set; } = new();
    }
}
