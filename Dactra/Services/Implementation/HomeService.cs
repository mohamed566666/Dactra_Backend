using Dactra.DTOs.RatingDTOs;

namespace Dactra.Services.Implementation
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        private readonly IFavoriteService _favoriteService;
        private readonly ILogger<HomeService> _logger;

        public HomeService(IHomeRepository homeRepository, IFavoriteService favoriteService, ILogger<HomeService> logger)
        {
            _homeRepository = homeRepository;
            _favoriteService = favoriteService;
            _logger = logger;
        }

        public async Task<IEnumerable<TopRatedDoctorDTO>> GetTopRatedDoctorsAsync(int count, int patientId = 0)
        {
            if (count < 1)
                throw new ArgumentException("at least one Doctor", nameof(count));

            if (count > 100)
                throw new ArgumentException("limit is up to 100", nameof(count));

            _logger.LogInformation("Fetching top {Count} rated doctors", count);

            var doctors = await _homeRepository.GetTopRatedDoctorsAsync(count);

            if (patientId > 0 && doctors.Any())
            {
                var doctorIds = doctors.Select(d => d.DoctorId).ToList();
                var favoriteIds = await _favoriteService.GetFavoriteServiceProviderIdsAsync(patientId, doctorIds);

                foreach (var doctor in doctors)
                {
                    doctor.IsFavorite = favoriteIds.Contains(doctor.DoctorId);
                }
            }

            return doctors;
        }
    }
}
