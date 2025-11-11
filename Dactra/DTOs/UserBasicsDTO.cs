namespace Dactra.DTOs
{
    public class UserBasicsDTO
    {
        public string UserName { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public bool IsVerified { get; set; }
    }
}
