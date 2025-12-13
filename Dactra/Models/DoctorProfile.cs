namespace Dactra.Models
{
    public class DoctorProfile : ServiceProviderProfile
    {

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        [Required]
        public int SpecializationId { get; set; }
        [ForeignKey(nameof(SpecializationId))]
        public Majors specialization { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [NotMapped]
        public int Age => CalculateAge(DateOfBirth);

        [Required]
        public DateTime StartingCareerDate { get; set; }

        [NotMapped]
        public int YearsOfExperience => CalculateYearsOfExperience(StartingCareerDate);

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<ScheduleTable> Time { get; set; } = new List<ScheduleTable>();
        public ICollection<DoctorQualification> Qualifications { get; set; }
        = new List<DoctorQualification>();

        private int CalculateAge(DateTime dob)
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }

        private int CalculateYearsOfExperience(DateTime start)
        {
            var today = DateTime.UtcNow.Date;
            var years = today.Year - start.Year;
            if (start.Date > today.AddYears(-years)) years--;
            return Math.Max(0, years);
        }
    }
}
