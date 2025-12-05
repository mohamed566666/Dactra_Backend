using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class SiteReviewRepository : ISiteReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public SiteReviewRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(SiteReview review)
        {
            await _context.SiteReviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<SiteReview?> GetByIdAsync(int id)
        {
            return await _context.SiteReviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<SiteReview?> GetByUserIdAsync(string userId)
        {
            return await _context.SiteReviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReviewerUserId == userId);
        }

        public async Task<IEnumerable<SiteReview>> GetAllAsync()
        {
            return await _context.SiteReviews
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(SiteReview review)
        {
            _context.SiteReviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.SiteReviews.FindAsync(id);
            if (existing != null)
            {
                _context.SiteReviews.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.SiteReviews.CountAsync();
        }

        public async Task<decimal> GetAverageScoreAsync()
        {
            var any = await _context.SiteReviews.AnyAsync();
            if (!any) return 0m;
            var avg = await _context.SiteReviews.AverageAsync(r => r.Score);
            return Math.Round((decimal)avg, 2);
        }
        public async Task<IEnumerable<(int Score, int Count)>> GetScoreCountsAsync()
        {
            var groups = await _context.SiteReviews
                .GroupBy(r => r.Score)
                .Select(g => new { Score = g.Key, Count = g.Count() })
                .ToListAsync();
            return groups.Select(g => (g.Score, g.Count));
        }
    }
}
