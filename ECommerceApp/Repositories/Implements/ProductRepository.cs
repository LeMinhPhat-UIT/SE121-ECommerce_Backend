using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeProductId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Products.AnyAsync(product =>
                product.Name.ToLower() == normalizedName &&
                (!excludeProductId.HasValue || product.Id != excludeProductId.Value));
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(product => product.Id == id && product.IsAvailable);
        }

        public async Task<Product?> GetByIdIncludingUnavailableAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(product => product.Id == id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Where(product => product.IsAvailable)
                .ToListAsync();
        }

        public async Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(product => product.CategoryId == categoryId && product.IsAvailable)
                .ToListAsync();
        }

        public IQueryable<Product> QueryAllAvailable()
        {
            return _context.Products
                .AsNoTracking()
                .Where(product => product.IsAvailable);
        }

        public IQueryable<Product> QueryByCategory(int categoryId)
        {
            return _context.Products
                .AsNoTracking()
                .Where(product => product.CategoryId == categoryId && product.IsAvailable);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}