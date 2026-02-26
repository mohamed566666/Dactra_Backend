
using System.Text.Json;

namespace Dactra.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobSetting _settings;
        private readonly IPatientProfileRepository _patientProfileRepository;

        public PaymentService(HttpClient httpClient,
            IOptions<PaymobSetting> options,
             IPatientProfileRepository patientProfileRepository)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _patientProfileRepository = patientProfileRepository;
        }
        public async Task<string> GetPaymentUrl(decimal amount, string email)
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
                        amount_cents = (int)(amount * 100),
                        currency = "EGP",
                        items = new object[] { }
                    });
                var orderData = await orderResponse.Content.ReadAsStreamAsync();

                using var doc2 = await JsonDocument.ParseAsync(orderData);
                var root2 = doc2.RootElement;

                int orderId = root2.GetProperty("id").GetInt32();
               


                var paymentKeyResponse = await _httpClient.PostAsJsonAsync(
                    "https://accept.paymob.com/api/acceptance/payment_keys",
                    new
                    {
                        auth_token = token,
                        amount_cents = (int)(amount * 100),
                        expiration = 3600,
                        order_id = orderId,
                        billing_data = new
                        {
                            email = email,
                            first_name = user.FirstName,
                            last_name = user.LastName,
                            phone_number = "01000000000",
                            city = "Cairo",
                            country = "EG",
                            street = "NA",
                            building = "NA",
                            floor = "NA",
                            apartment = "NA"
                        },
                        currency = "EGP",
                        integration_id = int.Parse(_settings.IntegrationId)
                    });

                var stream = await paymentKeyResponse.Content.ReadAsStreamAsync();
                using var doc3 = await JsonDocument.ParseAsync(stream);
                var root3 = doc3.RootElement;
                string paymentToken = root3.GetProperty("token").GetString() ?? throw new Exception("Payment token not found");
             
                

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
