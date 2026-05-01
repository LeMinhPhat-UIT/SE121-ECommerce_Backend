using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;

namespace ECommerceApp.Repositories.Implements
{
    public class CartItemRepository(ApplicationDbContext context) : ICartItemRepository
    {
        public void Add(CartItem cartItem)
        {
            context.CartItems.Add(cartItem);
        }

        public void Update(CartItem cartItem)
        {
            context.CartItems.Update(cartItem);
        }

        public void Remove(CartItem cartItem)
        {
            context.CartItems.Remove(cartItem);
        }

        public void RemoveRange(IEnumerable<CartItem> cartItems)
        {
            context.CartItems.RemoveRange(cartItems);
        }
    }
}