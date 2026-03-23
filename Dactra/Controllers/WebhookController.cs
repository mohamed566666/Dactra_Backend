﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dactra.DTOs.PaymobDto;

namespace Dactra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentWebhookController> _logger;

        public PaymentWebhookController(ApplicationDbContext context, IConfiguration configuration, IPaymentService paymentService, ILogger<PaymentWebhookController> logger)
        {
            _context = context;
            _configuration = configuration;
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Paymob webhook to update Payment, Appointment, and Slot.
        /// </summary>
        /// <param name="data">Webhook payload from Paymob</param>
        /// <returns>200 OK if processed</returns>
        [HttpPost("payment-processed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PaymobWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var json = JsonDocument.Parse(body);

                var obj = json.RootElement.GetProperty("obj");

                var orderId = obj.GetProperty("order").GetProperty("id").GetInt32();
                var success = obj.GetProperty("success").GetBoolean();
                return Ok(new { message = "Webhook received. Processing logic is currently disabled for testing." });
                //_logger.LogInformation("Webhook received.");
                //if (data == null || data.obj == null)
                //{
                //    _logger.LogWarning("Webhook data is null.");
                //    return BadRequest("Webhook data is null");
                //}
                //if (data?.obj?.order==null) { return BadRequest("order is null");    }
                //var orderId = data.obj.order.id;
                //var success = data.obj.success;
                //_logger.LogInformation("Processing webhook for OrderId={OrderId}, Success={Success}", orderId, success);

                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.PaymobOrderId == orderId.ToString());

                if (payment == null)
                {
                    _logger.LogWarning("Payment not found for OrderId={OrderId}", orderId);
                    return NotFound($"Payment with OrderId {orderId} not found");
                }
                var hmac = Request.Query["hmac"].FirstOrDefault()
                   ?? Request.Headers["hmac"].FirstOrDefault();

                _logger.LogInformation("HMAC received: {HMAC}", hmac);

                var flag =await _paymentService.ProcessPaymobCallbackAsync(json, hmac, CancellationToken.None);
                _logger.LogInformation("HMAC validation result: {Flag}", flag);



                if (success && flag)
                {
                    _logger.LogInformation("Updating DB for confirmed payment OrderId={OrderId}", orderId);

                    payment.Status = paymentStatus.Confirmed;
                    payment.PaymobTransactionId = obj.GetProperty("id").ToString();
                    var appointment = await _context.PatientAppointments
                        .FirstOrDefaultAsync(a => a.PaymentId == payment.Id);

                    if (appointment != null)
                    {
                        appointment.Status = AppointmentStatus.Confirmed;
                        _logger.LogInformation("Appointment {AppointmentId} confirmed.", appointment.Id);


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
                            _logger.LogWarning("Slot not found for AppointmentId={AppointmentId}", appointment.Id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Appointment not found for PaymentId={PaymentId}", payment.Id);
                    }

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("DB updated successfully for OrderId={OrderId}", orderId);

                }
                else
                {
                    payment.Status = paymentStatus.Failed;
                    await _context.SaveChangesAsync();
                    _logger.LogWarning("Payment failed or HMAC invalid for OrderId={OrderId}", orderId);

                }
                _logger.LogInformation("Webhook processing finished for OrderId={OrderId}", orderId);
                return Ok(new { message = "Webhook processed", orderId, success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Paymob webhook");
                return StatusCode(500, "Internal server error");
            }

       


          
        }
        [HttpGet("payment-result")]
        public IActionResult PaymentResult()
        {
            _logger.LogInformation("PaymentResult page accessed.");
            return Ok("Payment processed. You can close this page.");
        }

    }
}