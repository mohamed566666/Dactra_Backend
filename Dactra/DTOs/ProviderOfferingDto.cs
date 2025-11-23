namespace Dactra.DTOs
{
    public class ProviderOfferingDto
    {
        public int ProviderId { get; set; }
        public int TestServiceId { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
