namespace Dactra.DTOs.QuestionDTOs
{
    public class AnswererInfoDto
    {
        //public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string? Specialty { get; set; }
        public int? YearsOfExperience { get; set; }
        public bool IsDoctor { get; set; }
    }
}
