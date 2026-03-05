
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Dactra.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobSetting _settings;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(HttpClient httpClient,
            IOptions<PaymobSetting> options,
             IPatientProfileRepository patientProfileRepository,
             ApplicationDbContext context,
             ILogger<PaymentService> logger)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _patientProfileRepository = patientProfileRepository;
            _context = context;
            _logger = logger;
        }
        public async Task<string> GetPaymentUrl(Payment payment, string email)
        {
            try
            {
                var user = await _patientProfileRepository.GetByUserEmail(email);
                var authResponse = await _httpClient.PostAsJsonAsync(
                    "https://accept.paymob.com/api/auth/tokens",
                    new { api_key = _settings.ApiKey });

                var authData = await authResponse.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(authData);
                var root = doc.RootElement;
                string token = root.GetProperty("token").GetString();


                var orderResponse = await _httpClient.PostAsJsonAsync(
                    "https://accept.paymob.com/api/ecommerce/orders",
                    new
                    {
                        auth_token = token,
                        delivery_needed = false,
                        amount_cents = (int)(payment.Amount * 100),
                        currency = "EGP",
                        items = new object[] { }
                    });
                var orderData = await orderResponse.Content.ReadAsStreamAsync();

                using var doc2 = await JsonDocument.ParseAsync(orderData);
                var root2 = doc2.RootElement;

                int orderId = root2.GetProperty("id").GetInt32();
               

                payment.PaymobOrderId = orderId.ToString();
                await _context.SaveChangesAsync();

                var paymentKeyResponse = await _httpClient.PostAsJsonAsync(
                    "https://accept.paymob.com/api/acceptance/payment_keys",
                    new
                    {
                        auth_token = token,
                        amount_cents = (int)(payment.Amount * 100),
                        expiration = 3600,
                        order_id = orderId,
                        billing_data = new
                        {
                            email = email,
                            first_name = user.FirstName,
                            last_name = user.LastName,
                            phone_number =  "0000000000",
                            street = "N/A",
                            building = "N/A",
                            floor =  "N/A",
                            country = "EG",
                            apartment =  "N/A",
                            city =  "Cairo",

                        },
                        currency = "EGP",
                        integration_id = int.Parse(_settings.IntegrationId)
                    });

                var stream = await paymentKeyResponse.Content.ReadAsStreamAsync();
                var responseString = await paymentKeyResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Paymob /payment_keys response: {Response}", responseString);
                using var doc3 = await JsonDocument.ParseAsync(stream);
                
                if (!doc3.RootElement.TryGetProperty("token", out var paymentTokenElement))
                    throw new ApplicationException("Payment token not found in Paymob payment_key response");

                string paymentToken = paymentTokenElement.GetString() ?? throw new ApplicationException("Payment token is null");
              

                return $"https://accept.paymob.com/api/acceptance/iframes/1004947?payment_token={paymentToken}";
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new ApplicationException("An error occurred while processing the payment.", ex);
            }
        }
    }
}
