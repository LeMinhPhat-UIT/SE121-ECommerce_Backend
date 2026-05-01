using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces;

public interface ICancellationRepository
{
    Task<Cancellation?> GetByOrderIdAsync(int orderId, bool trackChanges = false);
    Task<Cancellation?> GetByIdAsync(int id, bool trackChanges = false);
    Task<Cancellation?> GetByIdWithFullOrderDetailsAsync(int id, bool trackChanges = false);
    Task<List<Cancellation>> GetAllWithOrderAsync(bool trackChanges = false);
    void Add(Cancellation cancellation);
}