namespace Dactra.DTOs.QuestionDTOs
{
    public class UpdateQuestionDto
    {
        [Required]
        [MinLength(10)]
        public string Content { get; set; } = string.Empty;
    }
}
