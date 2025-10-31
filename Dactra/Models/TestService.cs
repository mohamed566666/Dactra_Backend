namespace Dactra.Models
{
    public class TestService
    {
        public int testServiceId { get; set; }
        public string testServiceName { get; set; }
        public string testServiceDescription { get; set; }
        public ICollection<ProviderOffering> Offerings { get; set; } = new List<ProviderOffering>();
    }
}
