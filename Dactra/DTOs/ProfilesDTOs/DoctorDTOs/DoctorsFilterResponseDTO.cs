namespace Dactra.DTOs.ProfilesDTOs.DoctorDTOs
{
    public class DoctorsFilterResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public decimal AverageRating { get; set; }
        public string ? profileImageUrl { get; set; } = null;
        public decimal? OfflinePrice { get; set; }
        public decimal? OnlinePrice { get; set; }
    }
}
