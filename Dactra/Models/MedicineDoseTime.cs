namespace Dactra.Models
{
    public class MedicineDoseTime
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PrescriptionMedicineId { get; set; }

        [ForeignKey(nameof(PrescriptionMedicineId))]
        public PrescriptionMedicine PrescriptionMedicine { get; set; } = null!;

        [Required]
        public TimeSpan DoseTime { get; set; }

        [Required]
        public int DoseOrder { get; set; }
    }
}
