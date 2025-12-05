using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class MajorsRepository : GenericRepository<Majors>, IMajorsRepository
    {
        private readonly ApplicationDbContext _context;
        public MajorsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
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
