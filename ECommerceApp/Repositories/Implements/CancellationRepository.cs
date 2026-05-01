using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements;

public class CancellationRepository(ApplicationDbContext context) : ICancellationRepository
{
    public async Task<Cancellation?> GetByOrderIdAsync(int orderId, bool trackChanges = false)
    {
        var query = context.Cancellations.AsQueryable();
        if (!trackChanges) query = query.AsNoTracking();
        
        return await query.FirstOrDefaultAsync(c => c.OrderId == orderId);
    }

    public async Task<Cancellation?> GetByIdAsync(int id, bool trackChanges = false)
    {
        var query = context.Cancellations.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<Cancellation?> GetByIdWithFullOrderDetailsAsync(int id, bool trackChanges = false)
    {
        var query = context.Cancellations.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(c => c.Order)
                .ThenInclude(o => o.Customer)
            .Include(c => c.Order)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<List<Cancellation>> GetAllWithOrderAsync(bool trackChanges = false)
    {
        var query = context.Cancellations.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(c => c.Order) 
            .ToListAsync();
    }
    
    public void Add(Cancellation cancellation)
    {
        context.Cancellations.Add(cancellation);
    }
}