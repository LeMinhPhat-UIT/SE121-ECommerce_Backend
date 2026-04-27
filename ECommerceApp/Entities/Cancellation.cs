using ECommerceApp.Entities;
using ECommerceApp.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Entities
{
    public class Cancellation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        [Required(ErrorMessage = "Cancellation reason is required.")]
        [StringLength(500, ErrorMessage = "Cancellation reason cannot exceed 500 characters.")]
        public string Reason { get; set; } = null!;

        [Required]
        public CancellationStatus Status { get; set; }

        [Required]
        public DateTime RequestedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public int? ProcessedBy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OrderAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CancellationCharges { get; set; } = 0.00m;

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }

        public Refund Refund { get; set; } = null!;
    }
}