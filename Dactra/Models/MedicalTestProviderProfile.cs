using Dactra.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dactra.Models
{
    public class MedicalTestProviderProfile : ServiceProvider
    {
        public ServiceProvider Provider { get; set; } = null!;
        [Required(ErrorMessage = "Provider type is required")]
        public MedicalTestProviderType Type { get; set; }
        public ICollection<ProviderOffering> Offerings { get; set; } = new List<ProviderOffering>();
    }
}
