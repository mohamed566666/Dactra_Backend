namespace Dactra.DTOs.VitalSignDTOs
{
    public class VitalSignResponseDTO
    {
        public int VitalSignTypeId { get; set; }
        public decimal? Value { get; set; }
        public decimal? Value2 { get; set; }
        public DateTime date { get; set; }
    }
}
