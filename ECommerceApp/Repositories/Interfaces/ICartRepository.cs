using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetActiveCartByCustomerIdAsync(int customerId);
        Task<Cart?> GetByIdWithItemsAsync(int cartId);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task RemoveAsync(Cart cart);
    }
}