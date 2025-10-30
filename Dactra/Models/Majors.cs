namespace Dactra.Models
{
    public class Majors
    {
        public int Id { get; set; }
        public string  Name { get; set; } = string.Empty;
        public string Iconpath { get; set; } = string.Empty;
        public string Description { get; set; }= string.Empty;

       public List<Questions> questions { get; set; } = new List<Questions>();
        public List<Post> Post { get; set; } =new List<Post>();

    }
}
