namespace Dactra.DTOs.PostDTOs
{
    public class PostFeedResponseDto
    {
        public PagedResultDto<PostResponseDto> Posts { get; set; } = null!;
        public UserPostStatsDto Stats { get; set; } = null!;
    }
}
