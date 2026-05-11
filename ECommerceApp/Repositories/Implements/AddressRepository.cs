using System.Globalization;
using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class AddressRepository(ApplicationDbContext context) : IAddressRepository
    {
        public async Task<Address?> GetByIdAsync(int id, bool trackChanges = false)
        {
            var query = context.Addresses.AsQueryable();
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            
            return await query.FirstOrDefaultAsync(address => address.Id == id);
        }

        public async Task<Address?> GetByIdAndCustomerIdAsync(int addressId, int customerId, bool trackChanges = false)
        {
            var query = context.Addresses.AsQueryable();
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(address => address.Id == addressId && address.CustomerId == customerId);
        }

        public async Task<List<Address>> GetByCustomerIdAsync(int customerId, bool trackChanges = false)
        {
            var query = context.Addresses.AsQueryable();
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            
            return await query
                .Where(address => address.CustomerId == customerId)
                .ToListAsync();
        }

        
        public IQueryable<Address> QueryByCustomerId(int customerId)
        {
            return context.Addresses
                .AsNoTracking()
                .Where(address => address.CustomerId == customerId);
        }

        public void Add(Address address)
        {
            context.Addresses.Add(address);
        }

        public void Update(Address address)
        {
            context.Addresses.Update(address);
        }

        public void Remove(Address address)
        {
            context.Addresses.Remove(address);
        }
    }
}