using Dactra.DTOs.RatingDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IRatingService
    {
        Task<bool> RateProviderAsync(int patientId, int providerId,string heading, int score, string comment);
        Task<decimal> CalculateAverageRatingAsync(int providerId);
        Task<bool> UpdateRatingAsync(int patientId, int providerId,string heading, int score, string comment);
        Task<bool> DeleteRatingAsync(int patientId, int providerId);
        Task<List<RatingResponseDTO>> GetRatingsByPatientAsync(int patientId);
        Task<List<RatingResponseDTO>> GetRatingsforProviderAsync(int providerId);
        Task<RatingResponseDTO?> GetRatingByPatientAndProviderAsync(int patientId, int providerId);
        Task<ProviderRatingsSummaryDTO> GetRatingsSummaryForProviderAsync(int providerId);
    }
}
