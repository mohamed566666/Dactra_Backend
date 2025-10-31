namespace Dactra.Models
{
    public class Questions
    {
        public int  Id { get; set; }

        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Category { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int MajorsId { get; set; }
        public Majors Majors { get; set; }
    }
}
