using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface ICartItemRepository
    {
        void Add(CartItem cartItem);
        void Update(CartItem cartItem);
        void Remove(CartItem cartItem);
        void RemoveRange(IEnumerable<CartItem> cartItems);
    }
}