namespace Dactra.DTOs.PharmacyDto
{
    public class PharmacyRequestDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public List<PharmacyItemDto> items { get; set; }
    }
}
