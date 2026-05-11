using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address?> GetByIdAsync(int id, bool trackChanges = false);
        Task<Address?> GetByIdAndCustomerIdAsync(int addressId, int customerId, bool trackChanges = false);
        Task<List<Address>> GetByCustomerIdAsync(int customerId, bool trackChanges = false);
        
        IQueryable<Address> QueryByCustomerId(int customerId);
        
        void Add(Address address);
        void Update(Address address);
        void Remove(Address address);
    }
}