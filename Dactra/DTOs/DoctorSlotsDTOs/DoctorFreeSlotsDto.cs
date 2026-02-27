namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class DoctorFreeSlotsDto
    {
        public Dictionary<string, List<FreeSlotDto>> Slots { get; set; } = new();
    }
}
