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
    
    public async Task<Order?> GetByIdAndCustomerIdWithPaymentAsync(int orderId, int customerId, bool trackChanges = false)
    {
        var query = context.Orders.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId);
    }
    
    public async Task<Order?> GetOrderWithFullDetailsAsync(int orderId, bool trackChanges = false)
    {
        var query = context.Orders.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(o => o.Customer)
            .Include(o => o.BillingAddress)
            .Include(o => o.ShippingAddress)
            .Include(o => o.Payment)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> GetByIdAndCustomerIdAsync(int orderId, int customerId, bool trackChanges = false)
    {
        var query = context.Orders.AsQueryable();
        if (!trackChanges) query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId);
    }
    
    public async Task<bool> HasCustomerPurchasedAndReceivedProductAsync(int customerId, int productId)
    {
        return await context.Orders
            .Where(o => o.CustomerId == customerId && o.OrderStatus == OrderStatus.Delivered)
            .SelectMany(o => o.OrderItems)
            .AnyAsync(oi => oi.ProductId == productId);
    }
    
    public void Add(Order order)
    {
        context.Orders.Add(order);
    }
}