using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;

namespace Dactra.Services.Implementation
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        public HomeService(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }
        public async Task<IEnumerable<DTOs.TopRatedDoctorDTO>> GetTopRatedDoctorsAsync(int count = 10)
        {
            return await _homeRepository.GetTopRatedDoctorsAsync(count);
        }
    }
}
