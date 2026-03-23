using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private readonly string _hmacSecret;

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
            _hmacSecret = _configuration["Paymob:HmacSecret"];
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

        public async Task<bool> ProcessPaymobCallbackAsync(PaymobCallbackRequest callback, string hmacHeader, CancellationToken cancellationToken = default)
        {
            // 1. Verify HMAC
            try
            {
                if (string.IsNullOrEmpty(hmacHeader))
                {
                    _logger.LogWarning("Callback received without HMAC header");
                    return false;
                }

                var isValid = await VerifyCallbackAsync(callback, hmacHeader);

                if (!isValid)
                {
                    _logger.LogError("HMAC verification failed");
                    return false;
                }
                var orderId = callback.obj.order.id.ToString();
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymobOrderId == orderId, cancellationToken);
                if (payment == null)
                {
                    _logger.LogError("Payment not found for Order ID {OrderId}", orderId);
                    return false;
                }
                var callbackJson = JsonConvert.SerializeObject(callback);
                if (callback.obj.success)
                {
                    payment.Status = paymentStatus.Confirmed;
                    payment.PaymobTransactionId = callback.obj.id.ToString();
                    var appointment = await _context.PatientAppointments
                      .FirstOrDefaultAsync(a => a.PaymentId == payment.Id);

                    if (appointment != null)
                    {
                        appointment.Status = AppointmentStatus.Confirmed;
                        _logger.LogInformation("Appointment {AppointmentId} confirmed.", appointment.Id);
                    }
                    else {
                        
                        _logger.LogWarning("No appointment found for Payment {PaymentId}", payment.Id); 
                        return false;

                    }
                    var slot = await _context.DoctorAvailabilitySlots
                        .FirstOrDefaultAsync(s => s.Id == appointment.SlotId);

                    if (slot != null)
                    {
                        slot.IsBooked = true;
                        slot.IsReserved = false;
                        slot.ReservedUntil = null;
                        _logger.LogInformation("Slot {SlotId} updated to booked.", slot.Id);

                    }
                    else
                    {
                        _logger.LogWarning("No slot found for Appointment {AppointmentId}", appointment.Id);
                        return false;
                    }
                    await _context.SaveChangesAsync();

                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Paymob callback");
                return false;

            }
        }



        public async Task<bool> VerifyCallbackAsync(PaymobCallbackRequest callback, string hmacFromHeader)
        {
            try
            {

                var obj = callback.obj;

                var concatenatedString =
                    obj.amount_cents.ToString() +
                    obj.created_at +
                    obj.currency +
                    obj.error_occured.ToString().ToLower() +
                    obj.has_parent_transaction.ToString().ToLower() +
                    obj.id.ToString() +
                    obj.integration_id.ToString() +
                    obj.is_3d_secure.ToString().ToLower() +
                    obj.is_auth.ToString().ToLower() +
                    obj.is_capture.ToString().ToLower() +
                    obj.is_refunded.ToString().ToLower() +
                    obj.is_standalone_payment.ToString().ToLower() +
                    obj.is_voided.ToString().ToLower() +
                    obj.order.id.ToString() +
                    obj.owner.ToString() +
                    obj.pending.ToString().ToLower() +
                    (obj.source_data?.pan ?? "") +
                    (obj.source_data?.sub_type ?? "") +
                    (obj.source_data?.type ?? "") +
                    obj.success.ToString().ToLower();

                var computedHmac = ComputeHmac(concatenatedString, _hmacSecret);

                var isValid = computedHmac.Equals(hmacFromHeader, StringComparison.OrdinalIgnoreCase);

                if (!isValid)
                {
                    _logger.LogWarning(
                        "HMAC verification failed.\nString: {String}\nExpected: {Computed}\nReceived: {Received}",
                        concatenatedString,
                        computedHmac,
                        hmacFromHeader);
                }

                return (isValid);
            }

            catch (Exception ex)
            {

                return false;
            }


        }

        public string ComputeHmac(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}