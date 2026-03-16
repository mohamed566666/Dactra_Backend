namespace Dactra.DTOs.QuestionDTOs
{
    public class CreateQuestionDto
    {
        [Required]
        [MinLength(10)]
        public string Text { get; set; } = string.Empty;
    }
}
