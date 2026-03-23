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
        public async Task<IActionResult> PaymobWebhook([FromBody] PaymobCallbackRequest data)
        {
            try
            {
                var hmac = Request.Query["hmac"].FirstOrDefault()
                ?? Request.Headers["hmac"].FirstOrDefault()
                ?? Request.Form["hmac"].FirstOrDefault();
                _logger.LogInformation("HMAC received: {HMAC}", hmac);

                var flag =await _paymentService.ProcessPaymobCallbackAsync(data, hmac, CancellationToken.None);
                _logger.LogInformation("HMAC validation result: {Flag}", flag);
                return Ok(new { message = "Webhook processed"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Paymob webhook");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("payment-result")]
        public async Task <IActionResult> PaymentResult()
        {
            _logger.LogInformation("PaymentResult page accessed.");
            return Ok("Payment processed. You can close this page.");
        }

    }
}