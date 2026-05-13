using ECommerceApp.Commons;
using ECommerceApp.DTOs.OrderDTOs;

namespace ECommerceApp.Services.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<OrderResponse>> CreateOrderAsync(OrderCreateRequest orderDto);
    Task<ApiResponse<OrderResponse>> GetOrderByIdAsync(int orderId);
    Task<ApiResponse<ConfirmationResponse>> UpdateOrderStatusAsync(OrderStatusUpdateRequest statusDto);
    Task<ApiResponse<List<OrderResponse>>> GetAllOrdersAsync();
    Task<ApiResponse<List<OrderResponse>>> GetOrdersByCustomerAsync(int customerId);
}