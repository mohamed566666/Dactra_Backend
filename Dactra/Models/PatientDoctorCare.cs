namespace Dactra.Models
{
    public class PatientDoctorCare
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient { get; set; } = null!;

        [Required]
        public int DoctorId { get; set; }
        [ForeignKey(nameof(DoctorId))]
        public DoctorProfile Doctor { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAtUtc { get; set; } = DateTime.UtcNow.AddMonths(1);

        public bool IsActive { get; set; } = true;

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow > ExpiresAtUtc;

    }
}
