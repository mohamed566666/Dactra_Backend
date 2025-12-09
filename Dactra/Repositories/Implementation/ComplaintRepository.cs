namespace Dactra.Repositories.Implementation
{
    public class ComplaintRepository : GenericRepository<Complaint> , IComplaintRepository
    {
        public ComplaintRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Complaint>> GetAllAsync()
        {
            return await _context.Complaints
                .Include(c => c.User)
                .Include(c => c.Attachments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Complaint?> GetByIdAsync(int id)
        {
            return await _context.Complaints
                .Include(c => c.User)
                .Include(c => c.Attachments)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAttachmentsAsync(int complaintId, IEnumerable<ComplaintAttachment> attachments)
        {
            foreach (var a in attachments)
            {
                a.ComplaintId = complaintId;
                await _context.ComplaintAttachments.AddAsync(a);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAttachmentAsync(int attachmentId)
        {
            _context.ComplaintAttachments.RemoveRange(
                _context.ComplaintAttachments.Where(a => a.Id == attachmentId)
            );
            await _context.SaveChangesAsync();
        }

        public async Task<ComplaintAttachment?> GetAttachmentAsync(int attachmentId)
        {
            return await _context.ComplaintAttachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId);
        }

        public async Task<IEnumerable<Complaint?>> GetByUserIdAsync(string userId)
        {
            return await _context.Complaints
                .Where(c => c.UserId == userId)
                .Include(c => c.Attachments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
