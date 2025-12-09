namespace Dactra.Repositories.Implementation
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public HomeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TopRatedDoctorDTO>> GetTopRatedDoctorsAsync(int count = 10)
        {
            var topDoctors = await _dbContext.Doctors
                .Include(d => d.specialization)
                .Where(d => d.IsApproved == true)
                .OrderByDescending(d => d.Avg_Rating)
                .ThenByDescending(d => _dbContext.Ratings
                    .Count(r => r.ProviderId == d.Id))
                .Take(count)
                .Select(d => new TopRatedDoctorDTO
                {
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Specialization = d.specialization,
                    Avg_Rating = d.Avg_Rating,
                    TotalReviews = _dbContext.Ratings.Count(r => r.ProviderId == d.Id),
                    YearsOfExperience = d.YearsOfExperience,
                    Address = d.Address,
                })
                .ToListAsync();
            return topDoctors;
        }
    }
}
