namespace Dactra.DTOs.Sponsorship
{
    public class ReferPatientDTO
    {
        public int PatientId { get; set; }
        public int SponsorshipId { get; set; }
        public List<int> ProviderOfferingIds { get; set; } = new();
    }
}
