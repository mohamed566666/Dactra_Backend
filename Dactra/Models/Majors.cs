using System.ComponentModel.DataAnnotations;

namespace Dactra.Models
{
    public class Majors
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Major name is required")]
        public string  Name { get; set; } = string.Empty;
        public string Iconpath { get; set; } = string.Empty;
        public string Description { get; set; }= string.Empty;

       public List<Questions> Questions { get; set; } = new List<Questions>();
        public List<Post> Post { get; set; } =new List<Post>();

    }
}
