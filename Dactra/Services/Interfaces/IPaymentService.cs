namespace Dactra.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> GetPaymentUrl(decimal amount, string email);
    }
}
