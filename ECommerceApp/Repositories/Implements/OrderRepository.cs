using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Enums;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(int id, bool trackChanges = false)
    {
        var query = context.Orders.AsQueryable();
        if (!trackChanges) query = query.AsNoTracking();
    
        return await query.FirstOrDefaultAsync(o => o.Id == id);
    }
    
    public async Task<Order?> GetByIdWithDetailsAsync(int id, bool trackChanges = false)
    {
        var query = context.Orders.AsQueryable();
        
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
            .Include(o => o.BillingAddress)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Order>> GetAllWithDetailsAsync(bool trackChanges = false)
    {
        var query = context.Orders.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking(); 
        }

        return await query
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Customer)
            .Include(o => o.BillingAddress)
            .Include(o => o.ShippingAddress)
            .ToListAsync();
    }
    
    public async Task<List<Order>> GetByCustomerIdWithDetailsAsync(int customerId, bool trackChanges = false)
    {
        var query = context.Orders.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking(); 
        }

        return await query
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Customer)
            .Include(o => o.BillingAddress)
            .Include(o => o.ShippingAddress)
            .Where(o => o.CustomerId == customerId)
            .ToListAsync();
    }
    
    public void Add(Order order)
    {
        context.Orders.Add(order);
    }
}