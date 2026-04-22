namespace Dactra.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
        Task<List<Report>> GetAllAsync();
        Task<Report?> GetByIdAsync(int id);
        Task DeleteAsync(Report report);
        Task SaveChangesAsync();
    }
}
