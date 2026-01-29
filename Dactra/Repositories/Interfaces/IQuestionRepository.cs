namespace Dactra.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Questions>> GetAllAsync();
        Task<Questions?> GetByIdAsync(int id);
        Task AddAsync(Questions question);
        void Update(Questions question);
        Task SaveAsync();
    }
}
