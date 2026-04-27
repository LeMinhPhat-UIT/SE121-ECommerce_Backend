using ECommerceApp.Enums;
namespace ECommerceApp.DTOs.RefundDTOs
{
    public class PaymentGatewayRefundResponse
    {
        public bool IsSuccess { get; set; }
        public RefundStatus Status { get; set; }
        public string TransactionId { get; set; }
    }
}