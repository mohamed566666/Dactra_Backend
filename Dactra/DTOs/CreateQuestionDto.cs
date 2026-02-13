namespace Dactra.DTOs
{
    public class CreateQuestionDto
    {
        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public int MajorId { get; set; }
    }
}
