namespace Dactra.DTOs.RatingDTOs
{
    public class RatingResponseDTO
    {
        public int RatingId { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime RatedAt { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Heading { get; set; } = string.Empty;
    }
}
