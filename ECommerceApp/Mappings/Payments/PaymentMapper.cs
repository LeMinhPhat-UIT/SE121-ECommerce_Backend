using ECommerceApp.DTOs.PaymentDTOs;
using ECommerceApp.Entities;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Payments
{
    public interface IPaymentMapper
    {
        PaymentResponseDTO Map(Payment source);

        Payment Map(PaymentRequest source);
    }

    [Mapper]
    public partial class PaymentMapper : IPaymentMapper
    {
        public partial PaymentResponseDTO Map(Payment source);

        public partial Payment Map(PaymentRequest source);
    }
}