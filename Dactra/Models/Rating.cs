namespace Dactra.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient { get; set; } = null!;

        [Required]
        public int ProviderId { get; set; }

        [ForeignKey(nameof(ProviderId))]
        public ServiceProviderProfile Provider { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Rated_At { get; set; } = DateTime.Now;
    }
}
