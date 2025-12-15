namespace Dactra.DTOs.RatingDTOs
{
    public class ProviderRatingsSummaryDTO
    {
        public int TotalRatings { get; set; }
        public decimal AverageRating { get; set; }
        public Dictionary<int, int> ScoreCounts { get; set; } = new();
        public List<RatingResponseDTO> Ratings { get; set; } = new();
    }
}
