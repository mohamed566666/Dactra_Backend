namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class SlotItemWithIdDto
    {
        public int SlotId { get; set; }
        public DateTime SlotTime { get; set; }
        public bool IsBooked { get; set; } = false;
    }
}
