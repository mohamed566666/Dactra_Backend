namespace Dactra.Models
{
    public class DoctorProfile : ServiceProvider
    {
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public DateTime Starting_Career_Date { get; set; }
        public int Years_of_Experience { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
