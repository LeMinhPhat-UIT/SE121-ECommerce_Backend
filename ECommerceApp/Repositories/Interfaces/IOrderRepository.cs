using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, bool trackChanges = false);
    Task<Order?> GetByIdWithDetailsAsync(int id, bool trackChanges = false);
    Task<List<Order>> GetAllWithDetailsAsync(bool trackChanges = false);
    Task<List<Order>> GetByCustomerIdWithDetailsAsync(int customerId, bool trackChanges = false);
    Task<Order?> GetByIdAndCustomerIdWithPaymentAsync(int orderId, int customerId, bool trackChanges = false);
    Task<Order?> GetOrderWithFullDetailsAsync(int orderId, bool trackChanges = false);
    Task<Order?> GetByIdAndCustomerIdAsync(int orderId, int customerId, bool trackChanges = false);
    void Add(Order order);
}