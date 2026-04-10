namespace Dactra.DTOs.QuestionDTOs
{
    public class CreateAnswerDto
    {
        [Required]
        [MinLength(1)]
        public string Content { get; set; } = string.Empty;

        public int? ParentAnswerId { get; set; }
    }
}
