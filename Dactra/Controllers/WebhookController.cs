﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dactra.DTOs.PaymobDto;

namespace Dactra.Controllers
{
    [ApiController]
    [Route("api/payment/webhook")]
    [AllowAnonymous]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentWebhookController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> PaymobWebhook([FromBody] PaymobWebhookDto data)
        {
            if (data == null || data.Obj == null)
                return BadRequest("Webhook data is null");

            var orderId = data.Obj.Order.Id;
            var success = data.Obj.Success;

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymobOrderId == orderId.ToString());

            if (payment == null)
                return NotFound($"Payment with OrderId {orderId} not found");

            if (success)
            {
                payment.Status = paymentStatus.Confirmed;
                payment.PaymobTransactionId = data.Obj.Order.Transaction_Id;
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