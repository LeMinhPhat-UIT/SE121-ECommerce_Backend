using ECommerceApp.Commons;
using ECommerceApp.DTOs.RefundDTOs;
using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces;

public interface IRefundRepository
{
    Task<Refund?> GetByCancellationIdAsync(int cancellationId, bool trackChanges = false);
    Task<Refund?> GetByIdWithFullDetailsAsync(int id, bool trackChanges = false);
    Task<Refund?> GetByIdWithDetailsAsync(int id, bool trackChanges = false);
    Task<List<Refund>> GetAllWithDetailsAsync(bool trackChanges = false);
    void Add(Refund refund);
}