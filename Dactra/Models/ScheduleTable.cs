namespace Dactra.Models
{
    public class ScheduleTable
    {
        [Key]
        public int Id { get; set; }

        public List<PatientAppointment> Patient_Appointment { get; set; } = new List<PatientAppointment>();

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public DoctorProfile Doctor{ get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000, ErrorMessage = "Amount must be non-negative and reasonable.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [NotMapped]
        public List<bool> BookedStatuses
        {
            get
            {
                return Patient_Appointment.Select(d => d.Status).ToList();
            }
        }

    }
}
