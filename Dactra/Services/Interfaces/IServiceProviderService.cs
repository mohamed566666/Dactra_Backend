namespace Dactra.Services.Interfaces
{
    public interface IServiceProviderService
    {
        Task<bool> ApproveAsync(ServiceProviderType type, int id);
        Task<bool> DisApproveAsync(ServiceProviderType type, int id);
    }
}
