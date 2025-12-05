using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface ICityRepository
    {
        public Task AddCityAsync(City city);
        public Task<IEnumerable<City>> GetAllCitiesAsync();
        public Task<City?> GetCityByIdAsync(int id);
    }
}
