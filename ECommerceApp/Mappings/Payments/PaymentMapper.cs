using ECommerceApp.DTOs.PaymentDTOs;
using ECommerceApp.Entities;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Payments
{
    public interface IPaymentMapper
    {
        PaymentResponse Map(Payment source);

        Payment Map(PaymentRequest source);
    }

    [Mapper]
    public partial class PaymentMapper : IPaymentMapper
    {
        public partial PaymentResponse Map(Payment source);

        public partial Payment Map(PaymentRequest source);
    }
}