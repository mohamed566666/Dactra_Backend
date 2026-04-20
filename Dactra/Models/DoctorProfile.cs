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
        public TimeSpan? WorkingStartTime { get; set; }
        public TimeSpan? WorkingEndTime { get; set; }
        public int? ConsultationDurationMinutes { get; set; } = 30;
        public decimal? ConsultationPrice { get; set; } = 200.00m;

        [NotMapped]
        public int YearsOfExperience => CalculateYearsOfExperience(StartingCareerDate);
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<DoctorQualification> Qualifications { get; set; } = new List<DoctorQualification>();
        public ICollection<DoctorAvailabilitySlot> AvailableSlots { get; set; } = new List<DoctorAvailabilitySlot>();
        public ICollection<PatientDoctorCare> PatientCares { get; set; } = new List<PatientDoctorCare>();
        public ICollection<DoctorMedicalTestSponsor> MedicalTestSponsors { get; set; } = new List<DoctorMedicalTestSponsor>();
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

        [NotMapped]
        public bool HasValidWorkingHours =>
            WorkingStartTime.HasValue &&
            WorkingEndTime.HasValue &&
            WorkingStartTime.Value < WorkingEndTime.Value &&
            ConsultationDurationMinutes.HasValue &&
            ConsultationDurationMinutes > 0;
    }
}
