namespace Dactra.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post?> GetByIdAsync(int id);
        Task<Post> CreateAsync(Post post);
        Task<bool> UpdateAsync(int postId, int doctorId, UpdatePostDto dto);
        Task<bool> DeleteAsync(int postId, int doctorId);
    }
}
