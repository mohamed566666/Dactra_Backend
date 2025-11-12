using Dactra.Enums;
using Dactra.Models;

namespace Dactra.DTOs.ProfilesDTO
{
    public class DoctorCompleteDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime StartingCareerDate { get; set; }
        public string LicenceNo { get; set; }
        public string Address { get; set; }
        public int MajorId { get; set; }
    }
}
