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
