using Dactra.DTOs.RatingDTOs;

namespace Dactra.Repositories.Implementation
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _context;

        public HomeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TopRatedDoctorDTO>> GetTopRatedDoctorsAsync(int count)
        {
            count = Math.Clamp(count, 1, 100);

            return await _context.Doctors
                .Where(d => d.approvalStatus == ApprovalStatus.approved)
                .OrderByDescending(d => d.Avg_Rating)
                .Take(count)
                .Select(d => new TopRatedDoctorDTO
                {
                    DoctorId = d.Id,
                    DoctorName = d.FirstName + " " + d.LastName,
                    Specialization = d.specialization != null ? d.specialization.Name : "UnKnown",
                    ImageUrl = d.User.ImageUrl,
                    Rate = Math.Round(d.Avg_Rating, 2),
                    NumberOfRates = _context.Ratings.Count(r => r.ProviderId == d.Id)
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}