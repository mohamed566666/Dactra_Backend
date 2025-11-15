namespace Dactra.Models
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }= DateTime.UtcNow.AddMinutes(5);
        public bool IsUsed { get; set; } = false;
    }
}
