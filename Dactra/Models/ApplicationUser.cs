using Dactra.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dactra.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;
        public bool IsRegistrationComplete { get; set; } = false;
        public bool isDeleted { get; set; } = false;
    }
}
