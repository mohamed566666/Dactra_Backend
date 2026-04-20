using Dactra.DTOs.RatingDTOs;

namespace Dactra.Services.Implementation
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        private readonly ILogger<HomeService> _logger;

        public HomeService(IHomeRepository homeRepository, ILogger<HomeService> logger)
        {
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TopRatedDoctorDTO>> GetTopRatedDoctorsAsync(int count)
        {
            if (count < 1)
                throw new ArgumentException("at least one Doctor", nameof(count));

            if (count > 100)
                throw new ArgumentException("limit is up to 100", nameof(count));

            _logger.LogInformation("Fetching top {Count} rated doctors", count);

            return await _homeRepository.GetTopRatedDoctorsAsync(count);
        }
    }
}
