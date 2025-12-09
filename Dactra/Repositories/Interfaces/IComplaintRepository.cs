namespace Dactra.Repositories.Interfaces
{
    public interface IComplaintRepository : IGenericRepository<Complaint>
    {
        public Task AddAttachmentsAsync(int complaintId, IEnumerable<ComplaintAttachment> attachments);
        public Task<IEnumerable<Complaint?>> GetByUserIdAsync(string userId);
        Task<ComplaintAttachment?> GetAttachmentAsync(int attachmentId);
        public Task DeleteAttachmentAsync(int attachmentId);
    }
}
