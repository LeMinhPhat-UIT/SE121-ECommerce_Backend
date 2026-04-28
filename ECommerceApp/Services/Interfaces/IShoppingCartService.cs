using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ShoppingCartDTOs;

namespace ECommerceApp.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<ApiResponse<CartResponse>> GetCartByCustomerIdAsync(int customerId);
        Task<ApiResponse<CartResponse>> AddToCartAsync(AddToCartRequest addToCartDTO);
        Task<ApiResponse<CartResponse>> UpdateCartItemAsync(UpdateCartItemRequest updateCartItemDTO);
        Task<ApiResponse<CartResponse>> RemoveCartItemAsync(RemoveCartItemDTO removeCartItemDTO);
        Task<ApiResponse<ConfirmationResponse>> ClearCartAsync(int customerId);
    }
}