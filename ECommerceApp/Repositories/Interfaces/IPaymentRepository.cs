using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id, bool trackChanges = false);
    Task<Payment?> GetByOrderIdAsync(int orderId, bool trackChanges = false);
    Task<Payment?> GetByIdWithOrderAsync(int id, bool trackChanges = false);
    Task<Payment?> GetByIdAndOrderIdWithOrderAsync(int paymentId, int orderId, bool trackChanges = false);
    void Add(Payment payment);
}