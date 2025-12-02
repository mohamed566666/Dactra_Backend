using Dactra.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dactra.DTOs.ProfilesDTOs.PatientDTOs
{
    public class PatientCompleteDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public DateTime DateOfBirth { get; set; }
        public BloodTypes BloodType { get; set; }
        public SmokingStatus SmokingStatus { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string ChronicDisease { get; set; }
        public string Allergies { get; set; }
    }
}
