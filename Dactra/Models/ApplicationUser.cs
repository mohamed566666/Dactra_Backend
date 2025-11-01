using Dactra.Enums;

namespace Dactra.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string FirstName { get; set;}
        public string LastName { get; set;}
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; } = false;
        public Gender gender { get; set; }

    }
}
