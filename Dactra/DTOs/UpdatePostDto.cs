namespace Dactra.DTOs
{
    public class UpdatePostDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int MajorsId { get; set; }
    }
}
