namespace Dactra.DTOs.QuestionDTOs
{
    public class CreateQuestionDto
    {
        [Required]
        [MinLength(1)]
        public string Content { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
