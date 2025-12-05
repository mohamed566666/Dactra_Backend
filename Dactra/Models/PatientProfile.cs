using Dactra.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.Models
{
    public class PatientProfile : ProfileBase
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; }

        [Range(0, 250)]
        public int Height { get; set; }

        [Range(0, 500)]
        public int Weight { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [NotMapped]
        public int Age{get => CalculateAge(DateOfBirth);}
        public int? AddressId { get; set; }
        public City? Address { get; set; }
        public BloodTypes BloodType { get; set; }
        public ICollection<Medicines> Current_Medications { get; set; } = new List<Medicines>();
        public SmokingStatus SmokingStatus { get; set; }
        public string Allergies { get; set; } = string.Empty;
        public MaritalStatus MaritalStatus { get; set; }
        public string ChronicDisease { get; set; } = string.Empty;
        public List<VitalSign> VitalSign { get; set; } = new List<VitalSign>();
        public List<PatientAppointment> Patient_Appointment { get; set; } = new List<PatientAppointment>();
        public List<Questions> questions { get; set; } = new List<Questions>();

        private int CalculateAge(DateTime dob)
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
