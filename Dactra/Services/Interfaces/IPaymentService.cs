namespace Dactra.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> GetPaymentUrl(Payment payment, string email);
    }
}
