using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address?> GetByIdAsync(int id);
        Task<Address?> GetByIdAndCustomerIdAsync(int addressId, int customerId);
        Task<List<Address>> GetByCustomerIdAsync(int customerId);
        IQueryable<Address> QueryByCustomerId(int customerId);
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task RemoveAsync(Address address);
    }
}