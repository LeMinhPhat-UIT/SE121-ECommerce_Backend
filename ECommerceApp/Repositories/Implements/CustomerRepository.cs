using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class CustomerRepository(ApplicationDbContext context) : ICustomerRepository
    {
        public async Task<bool> ExistsByEmailAsync(string email, int? excludeCustomerId = null)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await context.Customers.AnyAsync(customer =>
                customer.Email.ToLower() == normalizedEmail &&
                (!excludeCustomerId.HasValue || customer.Id != excludeCustomerId.Value));
        }

        public async Task<Customer?> GetByIdAsync(int id, bool trackChanges = false)
        {
            var query = context.Customers.AsQueryable();
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            
            return await query.FirstOrDefaultAsync(customer => customer.Id == id);
        }

        public async Task<Customer?> GetActiveByIdAsync(int id, bool trackChanges = false)
        {
            var query = context.Customers.AsQueryable();
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            
            return await query.FirstOrDefaultAsync(customer => customer.Id == id && customer.IsActive);
        }

        public async Task<Customer?> GetByEmailAsync(string email, bool trackChanges = false)
        {
            var normalizedEmail = email.Trim().ToLower();
            var query = context.Customers.AsQueryable();
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            
            return await query.FirstOrDefaultAsync(customer => customer.Email.ToLower() == normalizedEmail);
        }

        public void Add(Customer customer)
        {
            context.Customers.Add(customer);
        }

        public void Update(Customer customer)
        {
            context.Customers.Update(customer);
        }
    }
}