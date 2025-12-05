using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CityRepository(ApplicationDbContext context)
        {
            _dbContext = context;
        }
        public async Task AddCityAsync(City city)
        {
            await _dbContext.Cities.AddAsync(city);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            return await _dbContext.Cities.ToListAsync();
        }

        public async Task<City?> GetCityByIdAsync(int id)
        {
            return await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
