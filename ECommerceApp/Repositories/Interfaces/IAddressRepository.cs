using ECommerceApp.Entites;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address?> GetByIdAsync(int id);
        Task<Address?> GetByIdAndCustomerIdAsync(int addressId, int customerId);
        Task<List<Address>> GetByCustomerIdAsync(int customerId);
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task RemoveAsync(Address address);
    }
}