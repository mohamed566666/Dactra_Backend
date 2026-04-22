using System.Numerics;

namespace Dactra.Models
{
    public class DoctorAvailabilitySlot
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DoctorProfile Doctor { get; set; } = null!;
        public DateTime SlotDateTimeUtc { get; set; }
        public bool IsBooked { get; set; } = false;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public int? AppointmentId { get; set; }
        public PatientAppointment? Appointment { get; set; }
        public bool IsReserved { get; set; } = false;
        public DateTime? ReservedUntil { get; set; }
        public SlotType SlotType { get; set; } = SlotType.InPerson;
    }
}
