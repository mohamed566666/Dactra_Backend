namespace Dactra.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int MajorsId { get; set; }
        public Majors Category { get; set; }
        public int DoctorId { get; set; }
        public DoctorProfile Doctor { get; set; }

    }
}
