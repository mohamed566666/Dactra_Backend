namespace Dactra.DTOs.ProfilesDTOs.DoctorDTOs
{
    public class DoctorUpdateDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public int SpecializationId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime StartingCareerDate { get; set; }
        public string Address { get; set; }
    }
}
