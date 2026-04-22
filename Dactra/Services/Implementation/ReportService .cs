namespace Dactra.Services.Implementation
{
    public class ReportService: IReportService
    {
        private readonly IReportRepository _repo;

        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }
        public async Task<Report> CreateReportAsync(Report report)
        {
            report.CreatedAt = DateTime.UtcNow;

            await _repo.AddAsync(report);
            await _repo.SaveChangesAsync();

            return report;
        }

        public async Task<List<ReportCardDTO>> GetReportCardsAsync()
        {
            var reports = await _repo.GetAllAsync();

            return reports.Select(r => new ReportCardDTO
            {
                Id = r.Id,
                UserEmail = r.User.Email,
                Title = r.Title,
                Content = r.Content,
                Type = r.Type.ToString(),
                RelatedEntityId = r.RelatedEntityId,
                RedirectUrl = GenerateRedirectUrl(r.Type, r.RelatedEntityId),
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _repo.GetByIdAsync(id);
            if (report == null) return false;

            await _repo.DeleteAsync(report);
            await _repo.SaveChangesAsync();

            return true;
        }

       
        private string GenerateRedirectUrl(ReportType type, int? id)
        {
            return type switch
            {
                ReportType.Post => $"/posts/{id}",
                ReportType.Comment => $"/posts/{id}",
                ReportType.Question => $"/questions/{id}",
                _ => "/"
            };
        }
    }
}
