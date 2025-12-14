namespace Dactra.DTOs.VitalSignDTOs
{
    public class VitalSignCreateDTO
    {
        [Required]
        public int VitalSignTypeId { get; set; }
        [Required]
        public int Value { get; set; }
        public int? Value2 { get; set; }
    }
}
