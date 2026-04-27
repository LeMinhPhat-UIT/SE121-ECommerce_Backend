using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeCustomerId = null)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await _context.Customers.AnyAsync(customer =>
                customer.Email.ToLower() == normalizedEmail &&
                (!excludeCustomerId.HasValue || customer.Id != excludeCustomerId.Value));
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(customer => customer.Id == id);
        }

        public async Task<Customer?> GetActiveByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(customer => customer.Id == id && customer.IsActive);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await _context.Customers.FirstOrDefaultAsync(customer => customer.Email.ToLower() == normalizedEmail);
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}