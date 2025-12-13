namespace Dactra.Models
{
    public class VitalSignType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public bool IsComposite { get; set; } = false;
        public string? CompositeFields { get; set; }
    }
}
