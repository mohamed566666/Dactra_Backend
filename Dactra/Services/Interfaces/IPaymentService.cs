using Dactra.DTOs.PaymobDto;
namespace Dactra.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> GetPaymentUrl(Payment payment, string email);
        Task<bool> RefundAppointmentAsync(int appointmentId);
         Task<bool> RefundPaymentAsync(int transactionId, decimal amount);
      Task< bool> ProcessPaymobCallbackAsync(JsonDocument json,string hmacHeader,CancellationToken cancellationToken = default);
        Task<bool> VerifyCallbackAsync(JsonDocument callback,string hmacFromHeader);
        string ComputeHmac(string data, string secret);
    }
}
