using ECommerceApp.DTOs.OrderDTOs;
using ECommerceApp.Entities;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Orders
{
    public interface IOrderMapper
    {
        OrderResponse Map(Order source);

        OrderItemResponse Map(OrderItem source);
    }

    [Mapper]
    public partial class OrderMapper : IOrderMapper
    {
        public partial OrderResponse Map(Order source);

        public partial OrderItemResponse Map(OrderItem source);
    }
}