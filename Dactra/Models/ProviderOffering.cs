namespace Dactra.Models
{
    public class ProviderOffering
    {
        public int Offer_ID { get; set; }

        public int Provider_ID { get; set; }
        public MedicalTestProviderProfile Provider { get; set; }

        public int TestService_ID { get; set; }
        public TestService TestService { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }

    }
}
