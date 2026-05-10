using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> ExistsByNameAsync(string name, int? excludeProductId = null);
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByIdIncludingUnavailableAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetByCategoryAsync(int categoryId);
        IQueryable<Product> QueryAllAvailable();
        IQueryable<Product> QueryByCategory(int categoryId);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
    }
}