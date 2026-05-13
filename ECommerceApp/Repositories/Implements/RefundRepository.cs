using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements;

public class RefundRepository(ApplicationDbContext context) : IRefundRepository
{
    public async Task<Refund?> GetByCancellationIdAsync(int cancellationId, bool trackChanges = false)
    {
        var query = context.Refunds.AsQueryable();
        if (!trackChanges) query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(r => r.CancellationId == cancellationId);
    }

    public async Task<Refund?> GetByIdWithFullDetailsAsync(int id, bool trackChanges = false)
    {
        var query = context.Refunds.AsQueryable();
        if (!trackChanges) query = query.AsNoTracking();

        return await query
            .Include(r => r.Cancellation)
            .ThenInclude(c => c.Order)
            .ThenInclude(o => o.Customer)
            .Include(r => r.Payment)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<Refund?> GetByIdWithDetailsAsync(int id, bool trackChanges = false)
    {
        var query = context.Refunds.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking(); 
        }

        return await query
            .Include(r => r.Cancellation)
            .ThenInclude(c => c.Order)
            .ThenInclude(o => o.Payment)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<List<Refund>> GetAllWithDetailsAsync(bool trackChanges = false)
    {
        var query = context.Refunds.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(r => r.Cancellation)
            .ThenInclude(c => c.Order)
            .ThenInclude(o => o.Payment)
            .ToListAsync();
    }
    
    public void Add(Refund refund)
    {
        context.Refunds.Add(refund);
    }
}