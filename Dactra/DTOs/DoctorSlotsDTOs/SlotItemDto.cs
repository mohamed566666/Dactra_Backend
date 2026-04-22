namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class SlotItemDto
    {
        public string SlotTime { get; set; } = string.Empty;
        public bool IsBooked { get; set; } = false;
        public SlotType SlotType { get; set; } = SlotType.InPerson;
    }
}
