using ECommerceApp.Data;
using ECommerceApp.Entites;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _context.Addresses.FirstOrDefaultAsync(address => address.Id == id);
        }

        public async Task<Address?> GetByIdAndCustomerIdAsync(int addressId, int customerId)
        {
            return await _context.Addresses.FirstOrDefaultAsync(address => address.Id == addressId && address.CustomerId == customerId);
        }

        public async Task<List<Address>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Addresses
                .AsNoTracking()
                .Where(address => address.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task AddAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Address address)
        {
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}