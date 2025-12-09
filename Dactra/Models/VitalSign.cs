namespace Dactra.Models
{
    public class VitalSign
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int  Value { get; set; }

        [Required]
        public string  Type { get; set; } = string.Empty;

        [Required]
        public string Note { get; set; }=string.Empty;

        [Required]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient { get; set; } = null!;
    }
}
