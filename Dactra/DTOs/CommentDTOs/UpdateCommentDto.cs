namespace Dactra.DTOs.CommentDTOs
{
    public class UpdateCommentDto
    {
        [Required]
        [MinLength(1)]
        public string Content { get; set; } = string.Empty;
    }
}
