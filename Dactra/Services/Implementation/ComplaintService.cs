using Dactra.DTOs.ComplaintsDTOs;
using Microsoft.AspNetCore.Identity;

namespace Dactra.Services.Implementation
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public ComplaintService(IComplaintRepository repo, UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _repo = repo;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task CreateAsync(string userId, CreateComplaintDTO dto)
        {
            var complaint = new Complaint
            {
                UserId = userId,
                Title = dto.Title,
                Content = dto.Content,
                Against= ComplaintAgainst.System,
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
            var userIds = complaints.Select(c => c.UserId).Distinct();

            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);

            return complaints.Select(c => Map(c, users));
            
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

            await _notificationService.SendAsync(complaint.UserId,"complaint solved","Complaint","Complaint",complaint.Id);
            _repo.Update(complaint);
            await _repo.SaveChangesAsync();
        }

        private static ComplaintResponseDTO Map(Complaint c) => new()
        {
            Id = c.Id,
            Title = c.Title,
            Against = c.Against,
            Content = c.Content,
            Status = c.status,
            CreatedAt = c.CreatedAt,
            ResolvedAt = c.ResolvedAt,
            AdminResponse = c.AdminResponse

        };

        private static ComplaintResponseDTO Map(Complaint c, Dictionary<string, string> users) => new()
        {
            Id = c.Id,
            Title = c.Title,
            Against = c.Against,
            Content = c.Content,
            Status = c.status,
            CreatedAt = c.CreatedAt,
            ResolvedAt = c.ResolvedAt,
            AdminResponse = c.AdminResponse,
            UserEmail = users.TryGetValue(c.UserId, out var email)
           ? email
           : null
        };
    }
}
