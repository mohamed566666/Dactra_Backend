namespace Dactra.DTOs.RatingDTOs
{
    public class UpdateRatingDTO
    {
        public string Heading { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
