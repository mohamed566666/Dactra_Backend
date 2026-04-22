namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class WorkingHoursResponseDTO
    {
        public WorkingHoursEntryDTO InPerson { get; set; } = new();
        public WorkingHoursEntryDTO Online { get; set; } = new();
    }
}
