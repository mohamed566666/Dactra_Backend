namespace Dactra.DTOs.PrescriptionDTOs
{
    public class DoseTimeResponseDto
    {
        public int DoseOrder { get; set; }
        public TimeSpan DoseTime { get; set; }
        public string DoseTimeDisplay { get; set; } = string.Empty;
    }
}
