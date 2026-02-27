namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class SlotResponseForAppointments
    {
        public int SlotId { get; set; }
        public string SlotTime { get; set; } = "05:00";
        public bool IsBooked { get; set; } = false;
    }
}
