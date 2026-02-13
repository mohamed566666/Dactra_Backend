namespace Dactra.DTOs
{
    public class CreateAnswerDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
