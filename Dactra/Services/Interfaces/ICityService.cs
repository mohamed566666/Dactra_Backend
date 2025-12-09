namespace Dactra.Services.Interfaces
{
    public interface ICityService
    {
        public Task AddCityAsync(string cityName);
        public Task<IEnumerable<City>> GetAllCitiesAsync();
        public Task<City?> GetCityByIdAsync(int id);
    }
}
