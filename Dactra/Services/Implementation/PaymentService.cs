
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

                var authData = await authResponse.Content.ReadFromJsonAsync<dynamic>();
                string token = authData.token;


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

                var orderData = await orderResponse.Content.ReadFromJsonAsync<dynamic>();
                int orderId = orderData.id;


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

                var paymentData = await paymentKeyResponse.Content.ReadFromJsonAsync<dynamic>();
                string paymentToken = paymentData.token;

                return $"https://accept.paymob.com/api/acceptance/iframes/{_settings.IframeId}?payment_token={paymentToken}";
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new ApplicationException("An error occurred while processing the payment.", ex);
            }
        }
    }
}
