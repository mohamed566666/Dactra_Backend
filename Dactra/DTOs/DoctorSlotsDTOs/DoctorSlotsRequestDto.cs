namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class DoctorSlotsRequestDto
    {
        public Dictionary<string, List<SlotItemDto>> Slots { get; set; } = new();
    }
}
