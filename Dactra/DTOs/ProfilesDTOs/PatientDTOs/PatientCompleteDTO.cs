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
        [Range(0, 1)]
        public Gender Gender { get; set; }
        [Range(40, 250)]
        public int Height { get; set; }
        [Range(20, 500)]
        public int Weight { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Range(0, 8)]
        public BloodTypes BloodType { get; set; }
        [Range(0, 3)]
        public SmokingStatus SmokingStatus { get; set; }
        [Range(0, 4)]
        public MaritalStatus MaritalStatus { get; set; }
        public List<int> AllergyIds { get; set; } = new();
        public List<int> ChronicDiseaseIds { get; set; } = new();
    }
}
