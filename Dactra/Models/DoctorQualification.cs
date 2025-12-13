namespace Dactra.Models
{
    public class DoctorQualification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorProfileId { get; set; }

        [ForeignKey(nameof(DoctorProfileId))]
        public DoctorProfile DoctorProfile { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
