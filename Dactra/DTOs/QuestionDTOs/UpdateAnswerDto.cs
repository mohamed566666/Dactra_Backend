namespace Dactra.DTOs.QuestionDTOs
{
    public class UpdateAnswerDto
    {
        [Required]
        [MinLength(10)]
        public string Content { get; set; } = string.Empty;
    }
}
