using ECommerceApp.Entities;
using ECommerceApp.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Entities
{
    public class Refund
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cancellation ID is required.")]
        public int CancellationId { get; set; }

        [ForeignKey("CancellationId")]
        public Cancellation Cancellation { get; set; } = null!;

        [Required(ErrorMessage = "Payment ID is required.")]
        public int PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; } = null!;

        [Range(0.01, 100000.00, ErrorMessage = "Refund amount must be between $0.01 and $100,000.00.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public RefundStatus Status { get; set; }

        [Required]
        public string RefundMethod { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Refund Reason cannot exceed 500 characters.")]
        public string? RefundReason { get; set; }

        [StringLength(100, ErrorMessage = "Transaction ID cannot exceed 100 characters.")]
        public string? TransactionId { get; set; }

        [Required]
        public DateTime InitiatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int? ProcessedBy { get; set; }
    }
}