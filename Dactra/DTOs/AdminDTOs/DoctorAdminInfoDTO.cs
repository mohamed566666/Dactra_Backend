namespace Dactra.DTOs.Admin
{
    public class DoctorAdminInfoDTO
    {
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ApprovalStatus? approvalStatus { get; set; }
    }
}
