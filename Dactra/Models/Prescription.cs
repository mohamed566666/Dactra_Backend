namespace Dactra.Models
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Diagnosis { get; set; } = string.Empty;

        [Required]
        public int AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public PatientAppointment Appointment { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PrescriptionMedicine> Medicines { get; set; } = new List<PrescriptionMedicine>();
    }
}
