namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class DoctorSlotsWithIdDto
    {
        public Dictionary<string, List<SlotItemWithIdDto>> Slots { get; set; } = new();
    }
}
