namespace ECommerceApp.DTOs.RefundDTOs
{
    public class PendingRefundResponse
    {
        public int CancellationId { get; set; }
        public int OrderId { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal CancellationCharge { get; set; }
        public decimal ComputedRefundAmount { get; set; }
        public string? CancellationRemarks { get; set; }
    }
}