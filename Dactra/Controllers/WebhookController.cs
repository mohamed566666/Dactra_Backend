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

        public PaymentWebhookController(ApplicationDbContext context, IConfiguration configuration, IPaymentService paymentService)
        {
            _context = context;
            _configuration = configuration;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Paymob webhook to update Payment, Appointment, and Slot.
        /// </summary>
        /// <param name="data">Webhook payload from Paymob</param>
        /// <returns>200 OK if processed</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PaymobWebhook([FromBody] PaymobCallbackRequest data)
        {
            if (data == null || data.obj == null)
                return BadRequest("Webhook data is null");

            var orderId = data.obj.order.id;
            var success = data.obj.success;

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymobOrderId == orderId.ToString());

            if (payment == null)
                return NotFound($"Payment with OrderId {orderId} not found");
            var hmac = Request.Query["hmac"].FirstOrDefault()
               ?? Request.Headers["hmac"].FirstOrDefault()
               ?? Request.Form["hmac"].FirstOrDefault();

            var flag= _paymentService.ProcessPaymobCallbackAsync(data, hmac, CancellationToken.None);

            if (success&&flag)
            {
                payment.Status = paymentStatus.Confirmed;
                payment.PaymobTransactionId = data.obj.id.ToString();
                var appointment = await _context.PatientAppointments
                    .FirstOrDefaultAsync(a => a.PaymentId == payment.Id);

                if (appointment != null)
                {
                    appointment.Status = AppointmentStatus.Confirmed;

                    var slot = await _context.DoctorAvailabilitySlots
                        .FirstOrDefaultAsync(s => s.Id == appointment.SlotId);

                    if (slot != null)
                    {
                        slot.IsBooked = true;
                        slot.IsReserved = false;
                        slot.ReservedUntil = null;
                    }
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                payment.Status = paymentStatus.Failed;
                await _context.SaveChangesAsync();
            }

            Console.WriteLine($"Webhook processed: OrderId={orderId}, Success={success}");

            return Ok(new { message = "Webhook processed", orderId, success });
        }
    }
}