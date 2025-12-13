namespace Dactra.DTOs
{
    public class CreateRatingDTO
    {
        [Range(1, 5)]
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
