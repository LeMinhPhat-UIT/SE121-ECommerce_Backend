using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class CartRepository(ApplicationDbContext context) : ICartRepository
    {
        public async Task<Cart?> GetActiveCartByCustomerIdAsync(int customerId, bool trackChanges = false)
        {
            var query = context.Carts.AsQueryable();
            if (!trackChanges) query = query.AsNoTracking();

            return await query
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.CustomerId == customerId && !cart.IsCheckedOut);
        }

        public async Task<Cart?> GetByIdWithItemsAsync(int cartId)
        {
            return await context.Carts
                .Include(cart => cart.CartItems)
                    .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.Id == cartId);
        }

        
        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public void Add(Cart cart)
        {
            context.Carts.Add(cart);
        }

        public void Update(Cart cart)
        {
            context.Carts.Update(cart);
        }

        public void Remove(Cart cart)
        {
            context.Carts.Remove(cart);
        }
    }
}