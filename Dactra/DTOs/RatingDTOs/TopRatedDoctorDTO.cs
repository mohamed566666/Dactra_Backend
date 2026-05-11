namespace Dactra.DTOs.RatingDTOs
{
    public class TopRatedDoctorDTO
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = null;
        public decimal Rate { get; set; }
        public int NumberOfRates { get; set; }
        public bool IsFavorite { get; set; } = false;
    }
}
