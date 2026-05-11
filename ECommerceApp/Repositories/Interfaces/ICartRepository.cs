using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetActiveCartByCustomerIdAsync(int customerId, bool trackChanges = false);
        Task<Cart?> GetByIdWithItemsAsync(int cartId);
        
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action);
        
        void Add(Cart cart);
        void Update(Cart cart);
        void Remove(Cart cart);
    }
}