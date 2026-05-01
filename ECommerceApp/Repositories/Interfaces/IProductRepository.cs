using ECommerceApp.Entites;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> ExistsByNameAsync(string name, int? excludeProductId = null);
        Task<Product?> GetByIdAsync(int id, bool trackChanges = false);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetByCategoryAsync(int categoryId);
        void Add(Product product);
        void Update(Product product);
    }
}