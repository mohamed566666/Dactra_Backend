using Dactra.DTOs.ProfilesDTOs.DoctorDTOs;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class DoctorProfileRepository : GenericRepository<DoctorProfile> , IDoctorProfileRepository
    {
        public DoctorProfileRepository(ApplicationDbContext context) : base(context)
        {

        }
        public override async Task<IEnumerable<DoctorProfile>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .ToListAsync();
        }
        public override async Task<DoctorProfile?> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DoctorProfile?> GetByUserEmail(string email)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.User.Email == email);
        }

        public async Task<DoctorProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }
        public async Task<(IEnumerable<DoctorProfile> doctors, int totalCount)> GetFilteredDoctorsAsync(DoctorFilterDTO filter)
        {
            var query = _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .AsQueryable();

            if (filter.SpecializationId.HasValue)
            {
                query = query.Where(d => d.SpecializationId == filter.SpecializationId.Value);
            }
            if (filter.Gender.HasValue)
            {
                query = query.Where(d => d.Gender == filter.Gender.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower().Trim();
                query = query.Where(d =>
                    (d.FirstName + " " + d.LastName).ToLower().Contains(searchTerm) ||
                    d.FirstName.ToLower().Contains(searchTerm) ||
                    d.LastName.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            if (filter.SortedByRating.HasValue && filter.SortedByRating.Value)
            {
                query = query.OrderByDescending(d => d.Avg_Rating);
            }
            else
            {
                query = query.OrderBy(d => d.FirstName); 
            }
            var doctors = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
            return (doctors, totalCount);
        }
    }
}
