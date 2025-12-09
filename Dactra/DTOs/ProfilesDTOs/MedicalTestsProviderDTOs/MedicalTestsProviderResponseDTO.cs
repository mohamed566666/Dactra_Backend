namespace Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs
{
    public class MedicalTestsProviderResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LicenceNo { get; set; }
        public string Address { get; set; }
        public string About { get; set; }
        public decimal Avg_Rating { get; set; }
        public MedicalTestProviderType Type { get; set; }
    }
}
