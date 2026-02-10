namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class DoctorSlotResponseDto
    {
        public int SlotId { get; set; }
        public DateTime SlotDateTimeUtc { get; set; }
        public bool IsBooked { get; set; }
    }
}
