using ECommerceApp.Entities;
using ECommerceApp.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = null!;

        [StringLength(50)]
        public string? TransactionId { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [StringLength(20)]
        public PaymentStatus Status { get; set; }

        public Refund Refund { get; set; } = null!;
    }
}