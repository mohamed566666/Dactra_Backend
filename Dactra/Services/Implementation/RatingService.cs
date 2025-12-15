
using Dactra.DTOs.RatingDTOs;

namespace Dactra.Services.Implementation
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IServiceProviderRepository _providerRepository;
        private readonly IMapper _mapper;
        public RatingService(IRatingRepository ratingRepository,IServiceProviderRepository providerRepository , IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _providerRepository = providerRepository;
            _mapper = mapper;
        }
        public async Task<bool> RateProviderAsync(int patientId, int providerId, string heading,int score, string comment)
        {
            var existingRating = await _ratingRepository.GetByPatientAndProviderAsync(patientId, providerId);
            if (existingRating != null)
                return false;
            var rating = new Rating
            {
                PatientId = patientId,
                ProviderId = providerId,
                Heading = heading,
                Score = score,
                Comment = comment
            };
            await _ratingRepository.AddAsync(rating);
            var provider = await _providerRepository.GetByIdAsync(providerId);
            if (provider != null)
            {
                provider.Avg_Rating = await CalculateAverageRatingAsync(providerId);
            }
            await _ratingRepository.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> CalculateAverageRatingAsync(int providerId)
        {
            var ratings = await _ratingRepository.GetByProviderIdAsync(providerId);
            if (!ratings.Any())
                return 0;
            return Math.Round((decimal)ratings.Average(r => r.Score), 2);
        }

        public async Task<bool> UpdateRatingAsync(int patientId, int providerId, string heading, int score, string comment)
        {
            var existingRating = await _ratingRepository.GetByPatientAndProviderAsync(patientId, providerId);
            if (existingRating == null) return false;
            existingRating.Score = score;
            existingRating.Comment = comment;
            existingRating.Heading = heading;
            await _ratingRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRatingAsync(int patientId, int providerId)
        { 
            var existingRating = await _ratingRepository.GetByPatientAndProviderAsync(patientId, providerId);
            if (existingRating == null) return false;
            _ratingRepository.Delete(existingRating);
            await _ratingRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<RatingResponseDTO>> GetRatingsByPatientAsync(int patientId)
        {
            var ratings = await _ratingRepository.GetByPatientIdAsync(patientId);
            return _mapper.Map<List<RatingResponseDTO>>(ratings);
        }

        public async Task<RatingResponseDTO?> GetRatingByPatientAndProviderAsync(int patientId, int providerId)
        {
            var rating = await _ratingRepository.GetByPatientAndProviderAsync(patientId, providerId);
            return rating == null ? null : _mapper.Map<RatingResponseDTO>(rating);
        }
        public async Task<List<RatingResponseDTO>> GetRatingsforProviderAsync(int providerId)
        {
            var ratings = await _ratingRepository.GetByProviderIdAsync(providerId);
            return _mapper.Map<List<RatingResponseDTO>>(ratings);
        }

        public async Task<ProviderRatingsSummaryDTO> GetRatingsSummaryForProviderAsync(int providerId)
        {
            var ratings = await _ratingRepository.GetByProviderIdAsync(providerId);
            var summary = new ProviderRatingsSummaryDTO
            {
                TotalRatings = ratings.Count,
                AverageRating = ratings.Any()
                    ? Math.Round((decimal)ratings.Average(r => r.Score), 2)
                    : 0,
                ScoreCounts = Enumerable.Range(1, 5).ToDictionary(score => score,score => ratings.Count(r => r.Score == score)),
                Ratings = _mapper.Map<List<RatingResponseDTO>>(ratings)
            };
            return summary;
        }
    }
}
