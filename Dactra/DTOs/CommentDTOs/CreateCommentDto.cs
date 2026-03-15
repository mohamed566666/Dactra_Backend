namespace Dactra.DTOs.CommentDTOs
{
    public class CreateCommentDto
    {
        [Required]
        [MinLength(1)]
        public string Content { get; set; } = string.Empty;
        public int? ParentCommentId { get; set; }
    }
}
