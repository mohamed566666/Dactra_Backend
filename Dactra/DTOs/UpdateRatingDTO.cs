namespace Dactra.DTOs
{
    public class UpdateRatingDTO
    {
        [Range(1, 5)]
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
