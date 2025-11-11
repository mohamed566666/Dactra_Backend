using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.Models
{
    public abstract class ProfileBase
    {
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
