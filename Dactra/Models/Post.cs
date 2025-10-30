namespace Dactra.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Category { get; set; } = string.Empty;

        public int MajorsId { get; set; }
        public Majors Majors { get; set; }
    }
}
