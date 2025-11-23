using Dactra.DTOs;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using System.Threading.Tasks;

namespace Dactra.Repositories.Implementation
{
    public class TestServiceRepository : ITestServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public TestServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<TestService>> GetAllAsync()
        {
            return await _context.TestServices
                .Include(x => x.Offerings)
                .ToListAsync();
        }

        public async Task<TestService?> GetByIdAsync(int id)
        {
            return await _context.TestServices
                .Include(x => x.Offerings)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TestService?> GetByNameAsync(string name)
        {
            return await _context.TestServices
                .Include(x => x.Offerings)
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }

        public async Task<TestService> CreateAsync(TestServiceDto dto)
        {
            var service = new TestService
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _context.TestServices.AddAsync(service);
            return service;
        }

        public async Task<bool> UpdateAsync(int id, TestServiceDto dto)
        {
            var service = await _context.TestServices.FindAsync(id);
            if (service == null)
                return false;

            service.Name = dto.Name;
            service.Description = dto.Description;

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var service = await _context.TestServices.FindAsync(id);
            if (service == null)
                return false;

            _context.TestServices.Remove(service);
            return true;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
