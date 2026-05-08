namespace Dactra.Models
{
    public class PatientFavoriteServiceProvider
    {
        [Key]
        [Column(Order = 0)]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient { get; set; }

        [Key]
        [Column(Order = 1)]
        public int ServiceProviderId { get; set; }

        [ForeignKey(nameof(ServiceProviderId))]
        public ServiceProviderProfile ServiceProvider { get; set; }
    }
}
