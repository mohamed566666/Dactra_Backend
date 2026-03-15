using Dactra.DTOs.TagDTOs;

namespace Dactra.Services.Interfaces
{
    public interface ITagService
    {
        Task<List<TagDto>> GetAllTagsAsync();
        Task<List<TagDto>> GetTagsByPostIdAsync(int postId);
        Task<List<TagDto>> AutoTagPostAsync(int postId, string content);
        Task<TagDto> CreateTagAsync(CreateTagDto dto);
        Task DeleteTagAsync(int id);
    }
}
