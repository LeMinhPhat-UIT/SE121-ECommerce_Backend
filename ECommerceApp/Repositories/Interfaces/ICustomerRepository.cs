using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> ExistsByEmailAsync(string email, int? excludeCustomerId = null);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetActiveByIdAsync(int id);
        Task<Customer?> GetByEmailAsync(string email);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
    }
}