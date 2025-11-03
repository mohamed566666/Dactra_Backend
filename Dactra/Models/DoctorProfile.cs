using Dactra.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.Models
{
    public class DoctorProfile : ServiceProvider
    {

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; }

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
