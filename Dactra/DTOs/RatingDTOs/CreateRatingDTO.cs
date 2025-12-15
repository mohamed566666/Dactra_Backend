namespace Dactra.DTOs.RatingDTOs
{
    public class CreateRatingDTO
    {
        [Range(1, 5)]
        public string Heading { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
