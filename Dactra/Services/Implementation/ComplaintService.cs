using Dactra.DTOs.ComplaintsDTOs;

namespace Dactra.Services.Implementation
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _repo;

        public ComplaintService(IComplaintRepository repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(string userId, CreateComplaintDTO dto)
        {
            var complaint = new Complaint
            {
                UserId = userId,
                Title = dto.Title,
                Content = dto.Content
            };

            await _repo.AddAsync(complaint);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ComplaintResponseDTO>> GetMyComplaintsAsync(string userId)
        {
            var complaints = await _repo.GetByUserIdAsync(userId);
            return complaints.Select(Map);
        }

        public async Task<IEnumerable<ComplaintResponseDTO>> GetAllComplaintsAsync()
        {
            var complaints = await _repo.GetAllWithAttachmentsAsync();
            return complaints.Select(Map);
        }

        public async Task<ComplaintResponseDTO> GetDetailsAsync(int id)
        {
            var complaint = await _repo.GetDetailsAsync(id)
                ?? throw new KeyNotFoundException("Complaint not found");

            return Map(complaint);
        }

        public async Task UpdateStatusAsync(int id, string adminId, UpdateComplaintStatusDTO dto)
        {
            var complaint = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Complaint not found");

            complaint.status = dto.Status;
            complaint.AdminResponse = dto.AdminResponse;
            complaint.AdminId = adminId;

            if (dto.Status == ComplaintStatus.Resolved)
                complaint.ResolvedAt = DateTime.UtcNow;

            _repo.Update(complaint);
            await _repo.SaveChangesAsync();
        }

        private static ComplaintResponseDTO Map(Complaint c) => new()
        {
            Id = c.Id,
            Title = c.Title,
            Content = c.Content,
            Status = c.status,
            CreatedAt = c.CreatedAt,
            ResolvedAt = c.ResolvedAt,
            AdminResponse = c.AdminResponse
        };
    }
}
