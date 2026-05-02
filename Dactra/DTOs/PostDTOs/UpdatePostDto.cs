namespace Dactra.DTOs.PostDTOs
{
    public class UpdatePostDto
    {
        [Required]
        [MinLength(1)]
        public string Content { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
