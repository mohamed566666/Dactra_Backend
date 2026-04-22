namespace Dactra.Services.Interfaces
{
    public interface IReportService
    {
        Task<Report> CreateReportAsync(Report report);
        Task<List<ReportCardDTO>> GetReportCardsAsync();
        Task<Report?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
