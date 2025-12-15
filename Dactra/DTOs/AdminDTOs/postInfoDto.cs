namespace Dactra.DTOs.Admin
{
    public class postInfoDto
    {
        public string id { get; set; }
        public string title   { get; set; }
        public string FullName   { get; set; }
        public DateTime createdAt   { get; set; }
        public bool isDeleted   { get; set; }
    }
}
