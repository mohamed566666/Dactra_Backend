namespace Dactra.DTOs.PrescriptionDTOs
{
    public class UpdatePrescriptionRequestDto
    {
        [Required]
        [MaxLength(2000)]
        public string Diagnosis { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<CreateMedicineDto> Medicines { get; set; } = new();
    }
}
