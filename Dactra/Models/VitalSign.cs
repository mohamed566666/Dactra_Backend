namespace Dactra.Models
{
    public class VitalSign
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int VitalSignTypeId { get; set; }
        [ForeignKey(nameof(VitalSignTypeId))]
        public VitalSignType Type { get; set; } = null!;
        [Required]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient { get; set; } = null!;

        [Required]
        public int Value { get; set; }
        public int? Value2 { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}
