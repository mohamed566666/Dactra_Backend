namespace Dactra.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000, ErrorMessage = "Amount must be non-negative and reasonable.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public string Currency { get; set; } = string.Empty;

        [Required]
        public string Method { get; set; } = "Paymob";

        public paymentStatus Status { get; set; }
        public string? PaymobOrderId { get; set; }

        public string? PaymobTransactionId { get; set; }

        public string? PaymobPaymentKey { get; set; }

        public bool IsHmacVerified { get; set; } = false;

        public DateTime? ExpiresAt { get; set; }
        public bool isRefunded { get; set; } = false;

        public List<PatientAppointment> PatientAppointments { get; set; } = new List<PatientAppointment>();
    }
}
