using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> ExistsByEmailAsync(string email, int? excludeCustomerId = null);
        Task<Customer?> GetByIdAsync(int id, bool trackChanges = false);
        Task<Customer?> GetActiveByIdAsync(int id, bool trackChanges = false);
        Task<Customer?> GetByEmailAsync(string email, bool trackChanges = false);
        void Add(Customer customer);
        void Update(Customer customer);
    }
}