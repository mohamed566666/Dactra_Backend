namespace Dactra.DTOs.CommentDTOs
{
    public class CommentLikeResponseDto
    {
        public int CommentId { get; set; }
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public DateTime? LikedAt { get; set; }
    }
}
