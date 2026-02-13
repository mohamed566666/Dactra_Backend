namespace Dactra.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post?> GetByIdAsync(int id);
        Task AddAsync(Post post);
        void Update(Post post);
        void SoftDelete(Post post);
        Task SaveChangesAsync();
    }
}
