using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class ComplaintRepository : GenericRepository<Complaint> , IComplaintRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ComplaintRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<Complaint>> GetAllAsync()
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.Attachments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Complaint?> GetByIdAsync(int id)
        {
            return await _dbContext.Complaints
                .Include(c => c.User)
                .Include(c => c.Attachments)
                .FirstOrDefaultAsync(c => c.Id == id);
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

        public async Task DeleteAttachmentAsync(int attachmentId)
        {
            _dbContext.ComplaintAttachments.RemoveRange(
                _dbContext.ComplaintAttachments.Where(a => a.Id == attachmentId)
            );
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ComplaintAttachment?> GetAttachmentAsync(int attachmentId)
        {
            return await _dbContext.ComplaintAttachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId);
        }

        public async Task<IEnumerable<Complaint?>> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Complaints
                .Where(c => c.UserId == userId)
                .Include(c => c.Attachments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
