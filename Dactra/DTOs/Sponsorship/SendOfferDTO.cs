namespace Dactra.DTOs.Sponsorship
{
    public class SendOfferDTO
    {
        public int DoctorId { get; set; }
        public string OfferContent { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
    }
}
