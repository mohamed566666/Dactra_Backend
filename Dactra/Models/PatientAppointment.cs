namespace Dactra.Models
{
    public class PatientAppointment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient { get; set; } = null!;
        public int? PrescriptionId { get; set; }
        [ForeignKey(nameof(PrescriptionId))]
        public Prescription? Prescription { get; set; }
        [Required]
        public int PaymentId { get; set; }
        [ForeignKey(nameof(PaymentId))]
        public Payment Payment { get; set; } = null!;
        [Required]
        public int SlotId { get; set; }
        [ForeignKey(nameof(SlotId))]
        public DoctorAvailabilitySlot Slot { get; set; } = null!;
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
        public bool IsReminderSent { get; set; } = false;
        public string? ReminderJobId { get; set; }
        [MaxLength(500)]
        public string? CancelledReason { get; set; }
    }
}
