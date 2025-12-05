using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface IComplaintRepository
    {
        public Task AddComplaintAsync(Complaint complaint);
        public Task AddAttachmentsAsync(int complaintId, IEnumerable<ComplaintAttachment> attachments);
        public Task<Complaint?> GetComplaintByIdAsync(int id);
        public Task<IEnumerable<Complaint>> GetAllComplaintsAsync();
        public Task<IEnumerable<Complaint?>> GetComplaintsByUserIdAsync(string userId);
        Task<ComplaintAttachment?> GetAttachmentAsync(int attachmentId);
        public Task UpdateComplaintAsync(Complaint complaint);
        public Task DeleteComplaintAsync(Complaint complaint);
        public Task DeleteAttachmentAsync(int attachmentId);
    }
}
