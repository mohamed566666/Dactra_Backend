using Dactra.Enums;

namespace Dactra.Models
{
    public class MedicalTestProviderProfile : ServiceProvider
    {
        public MedicalTestProviderType Type { get; set; }
        public ICollection<ProviderOffering> Offerings { get; set; } = new List<ProviderOffering>();
    }
}
