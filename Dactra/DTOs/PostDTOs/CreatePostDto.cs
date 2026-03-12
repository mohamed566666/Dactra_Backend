namespace Dactra.DTOs.PostDTOs
{
    public class CreatePostDto
    {
        [Required]
        [MinLength(1)]
        public string Content { get; set; } = string.Empty;
    }
}
