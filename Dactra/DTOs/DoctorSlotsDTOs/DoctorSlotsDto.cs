namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class DoctorSlotsDto
    {
        public Dictionary<string, List<SlotItemDto>> Slots { get; set; } = new();
    }
}
