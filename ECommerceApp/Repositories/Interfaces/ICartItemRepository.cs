using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface ICartItemRepository
    {
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task RemoveAsync(CartItem cartItem);
        Task RemoveRangeAsync(IEnumerable<CartItem> cartItems);
    }
}