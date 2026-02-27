namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class FreeSlotDto
    {
        public int SlotId { get; set; }
        public TimeOnly SlotTime { get; set; }
        public bool IsBooked { get; set; }
    }
}
