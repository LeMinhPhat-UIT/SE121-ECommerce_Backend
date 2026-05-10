using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeCategoryId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Categories.AnyAsync(category =>
                category.Name.ToLower() == normalizedName &&
                (!excludeCategoryId.HasValue || category.Id != excludeCategoryId.Value));
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Categories.AnyAsync(category => category.Id == id && category.IsActive);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(category => category.Id == id && category.IsActive);
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(category => category.IsActive)
                .ToListAsync();
        }

        public IQueryable<Category> QueryAllActive()
        {
            return _context.Categories
                .AsNoTracking()
                .Where(category => category.IsActive);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}