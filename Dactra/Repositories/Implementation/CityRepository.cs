using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class CityRepository : GenericRepository<City> , ICityRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CityRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;
        }
    }
}
