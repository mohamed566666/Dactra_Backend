using Dactra.Migrations;

namespace Dactra.Models
{
    public class ServiceProviderProfile : ProfileBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string LicenceNo { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        [Column(TypeName = "decimal(3,2)")]
        [Range(0, 5)]
        public decimal Avg_Rating { get; set; }

        public string About { get; set; } = string.Empty;
        public ApprovalStatus approvalStatus { get; set; } = ApprovalStatus.newPending;
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<LabsWorkingHour> WorkingHours { get; set; } = new List<LabsWorkingHour>();
        public ICollection<PatientFavoriteServiceProvider> PatientFavorites { get; set; } = new List<PatientFavoriteServiceProvider>();
    }
}
