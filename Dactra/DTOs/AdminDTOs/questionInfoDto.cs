namespace Dactra.DTOs.Admin
{
    public class questionInfoDto
    {
        public string id { get; set; }
        public  string  Content{ get; set; }
        public string  Pname { get; set; }
        public DateTimeOffset  createdAt { get; set; }
        public bool  isDeleted { get; set; }
    }
}
