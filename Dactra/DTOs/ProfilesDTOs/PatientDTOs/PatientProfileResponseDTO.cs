using Dactra.Enums;
using Dactra.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.DTOs.ProfilesDTOs.PatientDTOs
{
    public class PatientProfileResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public BloodTypes BloodType { get; set; }
        public bool IS_Smoking { get; set; }
        public string Allergies { get; set; }
        public string ChronicDisease { get; set; }
    }
}
