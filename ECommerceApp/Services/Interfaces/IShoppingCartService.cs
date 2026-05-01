using ECommerceApp.Commons;
using ECommerceApp.DTOs.ShoppingCartDTOs;

namespace ECommerceApp.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<ApiResponse<CartResponse>> GetCartByCustomerIdAsync(int customerId);
        Task<ApiResponse<CartResponse>> AddToCartAsync(AddToCartRequest addToCartDto);
        Task<ApiResponse<CartResponse>> UpdateCartItemAsync(UpdateCartItemRequest updateCartItemDto);
        Task<ApiResponse<CartResponse>> RemoveCartItemAsync(RemoveCartItemDTO removeCartItemDto);
        Task<ApiResponse<ConfirmationResponse>> ClearCartAsync(int customerId);
    }
}