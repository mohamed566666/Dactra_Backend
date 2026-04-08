namespace Dactra.Models
{
    public class MedicalTestProviderProfile : ServiceProviderProfile
    {
        [Required(ErrorMessage = "Provider type is required")]
        public MedicalTestProviderType Type { get; set; }
        [JsonIgnore]
        public ICollection<ProviderOffering> Offerings { get; set; } = new List<ProviderOffering>();
    }
}
