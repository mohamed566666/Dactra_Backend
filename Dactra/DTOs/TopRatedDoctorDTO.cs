namespace Dactra.DTOs
{
    public class TopRatedDoctorDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public Majors Specialization { get; set; }
        public decimal Avg_Rating { get; set; }
        public int TotalReviews { get; set; }
        public int YearsOfExperience { get; set; }
        public string Address { get; set; }
    }
}
