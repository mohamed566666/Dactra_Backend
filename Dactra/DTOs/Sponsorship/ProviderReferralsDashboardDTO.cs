namespace Dactra.DTOs.Sponsorship
{
    public class ProviderReferralsDashboardDTO
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Received { get; set; }
        public List<ProviderReferralItemDTO> Referrals { get; set; } = new();
    }
}
