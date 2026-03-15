namespace Dactra.Repositories.Interfaces
{
    public interface IPostTagRepository
    {
        Task<List<PostTag>> GetByPostIdAsync(int postId);
        Task AddRangeAsync(List<PostTag> postTags);
        Task RemoveByPostIdAsync(int postId);
        Task ReplaceTagsAsync(int postId, List<int> newTagIds);
    }
}
