namespace Dactra.Models
{
    public class QuestionAnswerLike
    {
        public int Id { get; set; }
        public int AnswerId { get; set; }
        public QuestionAnswer Answer { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
