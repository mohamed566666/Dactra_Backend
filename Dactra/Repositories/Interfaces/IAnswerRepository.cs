namespace Dactra.Repositories.Interfaces
{
    public interface IAnswerRepository
    {
        Task AddAsync(Answer answer);
        Task SaveAsync();
    }
}
