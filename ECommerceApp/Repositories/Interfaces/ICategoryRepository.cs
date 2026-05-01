using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsByNameAsync(string name, int? excludeCategoryId = null);
        Task<bool> ExistsByIdAsync(int id);
        Task<Category?> GetByIdAsync(int id, bool trackChanges = false);
        Task<List<Category>> GetAllAsync();
        void Add(Category category);
        void Update(Category category);
    }
}