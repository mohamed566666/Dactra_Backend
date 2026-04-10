namespace Dactra.DTOs.QuestionDTOs
{
    public class AnswerLikeResponseDto
    {
        public int AnswerId { get; set; }
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public DateTime? LikedAt { get; set; }
    }
}
