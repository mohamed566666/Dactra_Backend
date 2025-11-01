namespace Dactra.Models
{
    public class ServiceProvider
    {
        public int provider_Id { get; set; }
        public int User_ID { get; set; }
        public ApplicationUser User { get; set; }
        public string Name { get; set; }
        public string LicenceNo { get; set; }
        public string Address { get; set; }
        public decimal Avg_Rating { get; set; }
        public string About { get; set; }

    }
}
