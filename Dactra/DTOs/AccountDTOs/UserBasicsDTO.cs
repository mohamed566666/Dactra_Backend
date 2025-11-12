namespace Dactra.DTOs.AuthemticationDTOs
{
    public class UserBasicsDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public bool IsVerified { get; set; }
    }
}
