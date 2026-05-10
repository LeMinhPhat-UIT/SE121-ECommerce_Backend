using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetActiveCartByCustomerIdAsync(int customerId)
        {
            return await _context.Carts
                .Include(cart => cart.CartItems)
                    .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.CustomerId == customerId && !cart.IsCheckedOut);
        }

        public async Task<Cart?> GetByIdWithItemsAsync(int cartId)
        {
            return await _context.Carts
                .Include(cart => cart.CartItems)
                    .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.Id == cartId);
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

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

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}