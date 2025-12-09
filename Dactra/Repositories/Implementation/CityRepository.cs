namespace Dactra.Repositories.Implementation
{
    public class CityRepository : GenericRepository<City> , ICityRepository
    {
        public CityRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
