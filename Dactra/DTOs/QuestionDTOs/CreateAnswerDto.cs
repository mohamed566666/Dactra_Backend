namespace Dactra.DTOs.QuestionDTOs
{
    public class CreateAnswerDto
    {
        [Required]
        [MinLength(10)]
        public string Content { get; set; } = string.Empty;

        public int? ParentAnswerId { get; set; }
    }
}
