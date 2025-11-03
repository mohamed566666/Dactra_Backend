using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.Models
{
    public class ProviderOffering
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [ForeignKey(nameof(ProviderId))]
        public MedicalTestProviderProfile Provider { get; set; } = null!;

        [Required]
        public int TestServiceId { get; set; }

        [ForeignKey(nameof(TestServiceId))]
        public TestService TestService { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000)]
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }

    }
}
