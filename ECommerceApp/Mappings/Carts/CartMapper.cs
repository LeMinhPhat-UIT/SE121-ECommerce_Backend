using ECommerceApp.DTOs.ShoppingCartDTOs;
using ECommerceApp.Entities;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Carts
{
    public interface ICartMapper
    {
        CartResponse Map(Cart source);

        CartItemResponse Map(CartItem source);
    }

    [Mapper]
    public partial class CartMapper : ICartMapper
    {
        public partial CartResponse Map(Cart source);

        public partial CartItemResponse Map(CartItem source);
    }
}