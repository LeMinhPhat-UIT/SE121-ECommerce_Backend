using ECommerceApp.DTOs.RefundDTOs;
using ECommerceApp.Entities;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Refunds
{
    public interface IRefundMapper
    {
        RefundResponse Map(Refund source);

        PendingRefundResponse Map(Cancellation source);
    }

    [Mapper]
    public partial class RefundMapper : IRefundMapper
    {
        public partial RefundResponse Map(Refund source);

        public partial PendingRefundResponse Map(Cancellation source);
    }
}