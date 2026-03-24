namespace Dactra.DTOs.QuestionDTOs
{
    public class CreateQuestionDto
    {
        [Required]
        [MinLength(10)]
        public string Content { get; set; } = string.Empty;
    }
}
