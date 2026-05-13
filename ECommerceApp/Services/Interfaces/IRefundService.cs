using ECommerceApp.Commons;
using ECommerceApp.DTOs.RefundDTOs;

namespace ECommerceApp.Services.Interfaces;

public interface IRefundService
{
    Task<ApiResponse<List<PendingRefundResponse>>> GetEligibleRefundsAsync();
}