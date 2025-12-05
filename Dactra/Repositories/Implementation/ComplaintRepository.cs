using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ComplaintRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAttachmentsAsync(int complaintId, IEnumerable<ComplaintAttachment> attachments)
        {
            foreach (var a in attachments)
            {
                a.ComplaintId = complaintId;
                await _dbContext.ComplaintAttachments.AddAsync(a);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddComplaintAsync(Complaint complaint)
        {
            await _dbContext.Complaints.AddAsync(complaint);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAttachmentAsync(int attachmentId)
        {
            _dbContext.ComplaintAttachments.RemoveRange(
                _dbContext.ComplaintAttachments.Where(a => a.Id == attachmentId)
            );
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteComplaintAsync(Complaint complaint)
        {
            _dbContext.Complaints.Remove(complaint);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Complaint>> GetAllComplaintsAsync()
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.Attachments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<ComplaintAttachment?> GetAttachmentAsync(int attachmentId)
        {
            return await _dbContext.ComplaintAttachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId);
        }

        public async Task<Complaint?> GetComplaintByIdAsync(int id)
        {
            return await _dbContext.Complaints
                .Include(c => c.Attachments)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Complaint?>> GetComplaintsByUserIdAsync(string userId)
        {
            return await _dbContext.Complaints
                .Where(c => c.UserId == userId)
                .Include(c => c.Attachments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateComplaintAsync(Complaint complaint)
        {
            _dbContext.Complaints.Update(complaint);
            await _dbContext.SaveChangesAsync();
        }
    }
}
