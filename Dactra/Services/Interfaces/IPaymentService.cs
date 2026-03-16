using Dactra.DTOs.PaymobDto;
namespace Dactra.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> GetPaymentUrl(Payment payment, string email);
        Task<bool> RefundAppointmentAsync(int appointmentId);
         Task<bool> RefundPaymentAsync(int transactionId, decimal amount);
        bool ProcessPaymobCallbackAsync( PaymobCallbackRequest callback,string hmacHeader,CancellationToken cancellationToken = default);
        bool VerifyCallbackAsync(PaymobCallbackRequest callback,string hmacFromHeader);
        string ComputeHmac(string data, string secret);
    }
}
