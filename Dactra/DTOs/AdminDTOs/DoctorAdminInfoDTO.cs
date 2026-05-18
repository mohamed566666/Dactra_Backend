namespace Dactra.DTOs.Admin
{
    public class DoctorAdminInfoDTO
    {
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ApprovalStatus? approvalStatus { get; set; }
        public string LicenceNo { get; set; }
        public string imageUrl { get; set; }
    }
}
