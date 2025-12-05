using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Services.Implementation
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }
        public async Task AddCityAsync(string cityName)
        {
            var city = new City
            {
                Name = cityName
            };
            await _cityRepository.AddAsync(city);
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            var cities = await _cityRepository.GetAllAsync();
            return cities;
        }

        public async Task<City?> GetCityByIdAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            return city;
        }
    }
}
