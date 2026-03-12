namespace Dactra.DTOs.PostLikeDTOs
{
    public class PostLikeResponseDto
    {
        public int PostId { get; set; }
        public int TotalLikes { get; set; }
        public bool IsLiked { get; set; }
    }
}
