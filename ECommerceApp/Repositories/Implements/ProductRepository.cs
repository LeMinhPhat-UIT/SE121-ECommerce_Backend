using ECommerceApp.Data;
using ECommerceApp.Entites;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class ProductRepository(ApplicationDbContext context) : IProductRepository
    {
        public async Task<bool> ExistsByNameAsync(string name, int? excludeProductId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return await context.Products.AnyAsync(product =>
                product.Name.ToLower() == normalizedName &&
                (!excludeProductId.HasValue || product.Id != excludeProductId.Value));
        }

        public async Task<Product?> GetByIdAsync(int id, bool trackChanges = false)
        {
            var query = context.Products.AsQueryable();
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            
            return await query.FirstOrDefaultAsync(product => product.Id == id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
            return await context.Products
                .AsNoTracking()
                .Where(product => product.CategoryId == categoryId && product.IsAvailable)
                .ToListAsync();
        }

        public void Add(Product product)
        {
            context.Products.Add(product);
        }

        public void Update(Product product)
        {
            context.Products.Update(product);
        }
    }
}