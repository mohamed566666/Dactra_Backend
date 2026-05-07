namespace Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs
{
    public class MedicalTestProviderSearchFilterDTO : PaginationDto
    {
        public string? SearchTerm { get; set; }
        public MedicalTestProviderType? Type { get; set; }
    }
}
