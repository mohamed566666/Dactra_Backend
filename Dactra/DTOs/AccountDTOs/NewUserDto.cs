namespace Dactra.DTOs.AuthemticationDTOs
{
    public class NewUserDto
    {
        public string  Username  { get; set; }
        public string  Email  { get; set; }
        public string  Token  { get; set; }
        public Task<string> RefieshToken { get; set; }
        public bool IsRegistrationComplete { get; set; } = false;
    }
}
