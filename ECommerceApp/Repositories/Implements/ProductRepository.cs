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
        
        public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges = false)
        {
            var query = context.Products.AsQueryable();
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
    
            return await query.Where(p => ids.Contains(p.Id)).ToListAsync();
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
        
        public async Task<bool> DeductStockAsync(int productId, int quantity)
        {
            var rowsAffected = await context.Products
                .Where(p => p.Id == productId && p.StockQuantity >= quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.StockQuantity, p => p.StockQuantity - quantity));

            return rowsAffected > 0;
        }
    }
}