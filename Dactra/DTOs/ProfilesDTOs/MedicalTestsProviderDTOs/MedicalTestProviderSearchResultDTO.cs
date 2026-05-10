namespace Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs
{
    public class MedicalTestProviderSearchResultDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Rating { get; set; }
        public bool IsFavorite { get; set; } = false;
    }
}
