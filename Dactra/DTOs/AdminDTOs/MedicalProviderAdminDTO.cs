namespace Dactra.DTOs.Admin
{
    public class MedicalProviderAdminDTO
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsApproved { get; set; }
        public string Address { get; set; }
    }
}
