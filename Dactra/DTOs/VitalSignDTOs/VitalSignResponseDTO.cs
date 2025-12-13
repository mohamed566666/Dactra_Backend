namespace Dactra.DTOs.VitalSignDTOs
{
    public class VitalSignResponseDTO
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public decimal? Value { get; set; }
        public decimal? Value2 { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; }
    }
}
