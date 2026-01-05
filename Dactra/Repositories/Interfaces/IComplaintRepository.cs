namespace Dactra.Repositories.Interfaces
{
    public interface IComplaintRepository : IGenericRepository<Complaint>
    {
        Task<IEnumerable<Complaint>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Complaint>> GetAllWithAttachmentsAsync();
        Task<Complaint?> GetDetailsAsync(int id);
    }
}
