namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class SlotItemDto
    {
        public DateTime SlotTime { get; set; }
        public bool IsBooked { get; set; } = false;
    }
}
