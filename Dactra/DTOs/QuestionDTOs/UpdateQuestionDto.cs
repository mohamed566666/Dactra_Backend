namespace Dactra.DTOs.QuestionDTOs
{
    public class UpdateQuestionDto
    {
        [Required]
        [MinLength(10)]
        public string Text { get; set; } = string.Empty;
    }
}
