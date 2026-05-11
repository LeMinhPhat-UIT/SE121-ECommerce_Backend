using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements
{
    public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
    {
        public async Task<bool> ExistsByNameAsync(string name, int? excludeCategoryId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return await context.Categories.AnyAsync(category =>
                category.Name.ToLower() == normalizedName &&
                (!excludeCategoryId.HasValue || category.Id != excludeCategoryId.Value));
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await context.Categories.AnyAsync(category => category.Id == id && category.IsActive);
        }

        public async Task<Category?> GetByIdAsync(int id, bool trackChanges = false)
        {
            var query = context.Categories.AsQueryable();
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            
            return await query.FirstOrDefaultAsync(category => category.Id == id && category.IsActive);
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await context.Categories
                .AsNoTracking()
                .Where(category => category.IsActive)
                .ToListAsync();
        }

        public IQueryable<Category> QueryAllActive()
        {
            return context.Categories
                .AsNoTracking()
                .Where(category => category.IsActive);
        }

        public void Add(Category category)
        {
            context.Categories.Add(category);
        }

        public void Update(Category category)
        {
            context.Categories.Update(category);
        }
    }
}