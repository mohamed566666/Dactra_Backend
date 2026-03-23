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
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IConfiguration _configuration;

        public PaymentService(
            HttpClient httpClient,
            IOptions<PaymobSetting> options,
            IPatientProfileRepository patientProfileRepository,
            ApplicationDbContext context,
            ILogger<PaymentService> logger,
            IAppointmentRepository appointmentRepository,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _patientProfileRepository = patientProfileRepository;
            _context = context;
            _logger = logger;
            _appointmentRepository = appointmentRepository;
            _configuration = configuration;
        }

        public async Task<string> GetPaymentUrl(Payment payment, string email)
        {
            try
            {

                var user = await _patientProfileRepository.GetByUserEmail(email);


                var authResponse = await _httpClient.PostAsJsonAsync(
                    "https://accept.paymob.com/api/auth/tokens",
                    new { api_key = _settings.ApiKey });

                var authString = await authResponse.Content.ReadAsStringAsync();
                using var authDoc = JsonDocument.Parse(authString);
                string token = authDoc.RootElement.GetProperty("token").GetString()
                               ?? throw new ApplicationException("Paymob auth token not found");


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

                var orderString = await orderResponse.Content.ReadAsStringAsync();
                using var orderDoc = JsonDocument.Parse(orderString);
                int orderId = orderDoc.RootElement.GetProperty("id").GetInt32();

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
                            phone_number = "0000000000",
                            street = "N/A",
                            building = "N/A",
                            floor = "N/A",
                            country = "EG",
                            apartment = "N/A",
                            city = "Cairo"
                        },
                        currency = "EGP",
                        integration_id = int.Parse(_settings.IntegrationId)
                    });
                var captureResponse = await _httpClient.PostAsJsonAsync(
               "https://accept.paymob.com/api/acceptance/capture",
               new
               {
                   auth_token = token,
                   transaction_id = int.Parse(_settings.IntegrationId)
,
                   amount_cents = (int)(payment.Amount * 100)
               });
                var paymentKeyString = await paymentKeyResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Paymob /payment_keys response: {Response}", paymentKeyString);

                using var paymentKeyDoc = JsonDocument.Parse(paymentKeyString);
                if (!paymentKeyDoc.RootElement.TryGetProperty("token", out var paymentTokenElement))
                    throw new ApplicationException("Payment token not found in Paymob payment_key response");

                string paymentToken = paymentTokenElement.GetString()
                                      ?? throw new ApplicationException("Payment token is null");


                string iframeUrl = _settings.IframeId.Replace("{payment_key_obtained_previously}", paymentToken);
                return iframeUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Paymob payment URL");
                throw new ApplicationException("An error occurred while processing the payment.", ex);
            }
        }
        public async Task<bool> RefundPaymentAsync(int transactionId, decimal amount)
        {
            try
            {

                var authResponse = await _httpClient.PostAsJsonAsync(
                    "https://accept.paymob.com/api/auth/tokens",
                    new { api_key = _settings.ApiKey });

                var authString = await authResponse.Content.ReadAsStringAsync();
                using var authDoc = JsonDocument.Parse(authString);
                string token = authDoc.RootElement.GetProperty("token").GetString()
                               ?? throw new ApplicationException("Paymob auth token not found");

                int amountCents = (int)(amount * 100);


                var captureResponse = await _httpClient.PostAsJsonAsync(
                    "https://accept.paymob.com/api/acceptance/capture",
                    new
                    {
                        auth_token = token,
                        transaction_id = transactionId,
                        amount_cents = amountCents
                    });

                var captureResult = await captureResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Paymob Capture response: {Response}", captureResult);


                var refundResponse = await _httpClient.PostAsJsonAsync(
                    "https://accept.paymob.com/api/acceptance/void_refund/refund",
                    new
                    {
                        auth_token = token,
                        transaction_id = transactionId,
                        amount_cents = amountCents
                    });

                var refundResult = await refundResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Paymob Refund response: {Response}", refundResult);

                using var refundDoc = JsonDocument.Parse(refundResult);
                bool success = refundDoc.RootElement.TryGetProperty("success", out var successProp)
                               && successProp.GetBoolean();

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Capture + Refund");
                return false;
            }
        }
        public async Task<bool> RefundAppointmentAsync(int slotid)
        {

            var payments = await _context.Payments
              .Include(p => p.PatientAppointments)
              .ThenInclude(pa => pa.Slot)
               .ToListAsync();

            bool allSuccess = true;
            try
            {

                var refundpayment = payments.FirstOrDefault(p => p.PatientAppointments.Any(pa => pa.SlotId == slotid));
                if (refundpayment == null)
                {
                    _logger.LogInformation("Payment {PaymentId} does not include appointment {AppointmentId}, skipping refund.", refundpayment.Id, slotid);

                }
                // 1️⃣ Capture + Refund
                bool success = await RefundPaymentAsync(int.Parse(refundpayment.PaymobTransactionId), refundpayment.Amount);

                if (success)
                {
                    refundpayment.isRefunded = true;
                    _context.Update(refundpayment);
                }
                else
                {
                    allSuccess = false;
                    _logger.LogWarning("Refund failed for TransactionId {TransactionId}", refundpayment.PaymobTransactionId);
                }
            }
            catch (Exception ex)
            {
                allSuccess = false;
                _logger.LogError(ex, "Exception during refund for TransactionId {TransactionId}");
            }


            await _context.SaveChangesAsync();
            return allSuccess;
        }

        public async Task<bool> ProcessPaymobCallbackAsync(JsonDocument json, string hmacHeader, CancellationToken cancellationToken = default)
        {
            // 1. Verify HMAC
            if (string.IsNullOrEmpty(hmacHeader))
            {
                _logger.LogWarning("Callback received without HMAC header");
                return false;
            }

            var isValid =await VerifyCallbackAsync(json, hmacHeader);

            if (!isValid)
            {
                _logger.LogError("HMAC verification failed");
                return false;
            }
            return true;

        }



        public Task<bool> VerifyCallbackAsync(JsonDocument callback, string hmacFromHeader)
        {
            try
            {
                var obj = callback.RootElement.GetProperty("obj");

                var concatenatedString =
                    obj.GetProperty("amount_cents").GetInt64().ToString() +
                    obj.GetProperty("created_at").GetString() +
                    obj.GetProperty("currency").GetString() +
                    obj.GetProperty("error_occured").GetBoolean().ToString().ToLower() +
                    obj.GetProperty("has_parent_transaction").GetBoolean().ToString().ToLower() +
                    obj.GetProperty("id").GetInt32().ToString() +
                    obj.GetProperty("integration_id").GetInt32().ToString() +
                    obj.GetProperty("is_3d_secure").GetBoolean().ToString().ToLower() +
                    obj.GetProperty("is_auth").GetBoolean().ToString().ToLower() +
                    obj.GetProperty("is_capture").GetBoolean().ToString().ToLower() +
                    obj.GetProperty("is_refunded").GetBoolean().ToString().ToLower() +
                    obj.GetProperty("is_standalone_payment").GetBoolean().ToString().ToLower() +
                    obj.GetProperty("is_voided").GetBoolean().ToString().ToLower() +
                    obj.GetProperty("order").GetProperty("id").GetInt32().ToString() +
                    obj.GetProperty("owner").GetInt32().ToString() +
                    obj.GetProperty("pending").GetBoolean().ToString().ToLower() +
                    (obj.GetProperty("source_data").TryGetProperty("pan", out var pan) ? pan.GetString() : "") +
                    (obj.GetProperty("source_data").TryGetProperty("sub_type", out var subType) ? subType.GetString() : "") +
                    (obj.GetProperty("source_data").TryGetProperty("type", out var type) ? type.GetString() : "") +
                    obj.GetProperty("success").GetBoolean().ToString().ToLower();

                var computedHmac = ComputeHmac(concatenatedString, _configuration["Paymob:HmacSecret"]);
              
                var isValid = computedHmac.Equals(hmacFromHeader, StringComparison.OrdinalIgnoreCase);

                if (!isValid)
                {
                    _logger.LogWarning(
                        "HMAC verification failed.\nString: {String}\nExpected: {Computed}\nReceived: {Received}",
                        concatenatedString,
                        computedHmac,
                        hmacFromHeader);
                }

                return Task.FromResult(isValid);
            }

            catch (Exception ex)
            {

                return Task.FromResult(false);
            }


        }

        public string ComputeHmac(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);

            using var hmac = new HMACSHA512(keyBytes);

            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

            return BitConverter.ToString(hash)
                .Replace("-", "")
                .ToLower();
        }
    }
}