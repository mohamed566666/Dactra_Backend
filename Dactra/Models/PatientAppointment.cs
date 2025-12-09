namespace Dactra.Models
{
    public class PatientAppointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient{ get; set; } = null!;

        [Required]
        public int PrescriptionId { get; set; }

        [ForeignKey(nameof(PrescriptionId))]
        public Prescription Prescription { get; set; } = null!;

        [Required]
        public int PaymentId { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public Payment Payment { get; set; } = null!;

        [Required]
        public int ScheduleTableId { get; set; }

        [ForeignKey(nameof(ScheduleTableId))]
        public ScheduleTable Schedule_table { get; set; } = null!;


        public bool Status { get; set; } = false;
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;

    }
}
