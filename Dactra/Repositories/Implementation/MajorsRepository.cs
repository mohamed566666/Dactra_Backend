using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class MajorsRepository : IMajorsRepository
    {
        private readonly ApplicationDbContext _context;
        public MajorsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Majors major)
        {
            await _context.Majors.AddAsync(major);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Majors major)
        {
            _context.Majors.Remove(major);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Majors>> GetAllAsync()
        {
            return await _context.Majors.ToListAsync();
        }

        public async Task<Majors> GetByIdAsync(int id)
        {
            return await _context.Majors.FindAsync(id);
        }

        public async Task UpdateAsync(Majors major)
        {
            _context.Majors.Update(major);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateIcon(int id, string iconUrl)
        {
            var major = await _context.Majors.FindAsync(id);
            if (major != null)
            {
                major.Iconpath = iconUrl;
                _context.Majors.Update(major);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Major with ID {id} not found.");
            }
        }
    }
}
