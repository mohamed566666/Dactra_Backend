namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class SlotResponseForAppointments
    {
        public int SlotId { get; set; }
        public DateTime SlotTime { get; set; }
        public bool IsBooked { get; set; } = false;
    }
}
