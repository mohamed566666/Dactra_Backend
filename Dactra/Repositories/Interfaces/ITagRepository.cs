namespace Dactra.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(int id);
        Task<Tag?> GetByNameAsync(string name);
        Task<List<Tag>> GetByIdsAsync(List<int> ids);
        Task<List<Tag>> GetByNamesAsync(List<string> names);
        Task<Tag> CreateAsync(Tag tag);
        Task DeleteAsync(int id);

    }
}
