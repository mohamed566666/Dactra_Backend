namespace Dactra.DTOs
{
    public class VerficationsDTP
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
}
