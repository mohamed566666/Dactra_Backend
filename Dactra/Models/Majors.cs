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

       public List<Question> Questions { get; set; } = new List<Question>();
        public List<Post> Post { get; set; } =new List<Post>();

    }
}
