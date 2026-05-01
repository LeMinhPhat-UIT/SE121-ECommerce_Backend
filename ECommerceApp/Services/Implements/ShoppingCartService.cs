using ECommerceApp.Commons;
using ECommerceApp.DTOs.ShoppingCartDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Carts;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements;

public class ShoppingCartService(
    IUnitOfWork unitOfWork,
    ICartMapper mapper) : IShoppingCartService
{
    public async Task<ApiResponse<CartResponse>> GetCartByCustomerIdAsync(int customerId)
    {
        try
        {
            var cart = await unitOfWork.CartRepository.GetActiveCartByCustomerIdAsync(customerId);
            if (cart == null)
            {
                return new ApiResponse<CartResponse>(200, CreateEmptyCartResponse(customerId));
            }

            return new ApiResponse<CartResponse>(200, MapCartToDto(cart));
        }
        catch (Exception ex)
        {
            return new ApiResponse<CartResponse>(500, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CartResponse>> AddToCartAsync(AddToCartRequest addToCartDto)
    {
        try
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(addToCartDto.ProductId);
            if (product == null) return new ApiResponse<CartResponse>(404, "Product not found.");

            if (addToCartDto.Quantity > product.StockQuantity)
                return new ApiResponse<CartResponse>(400, $"Only {product.StockQuantity} units available.");

            var cart = await unitOfWork.CartRepository.GetActiveCartByCustomerIdAsync(addToCartDto.CustomerId, trackChanges: true);

            if (cart == null)
            {
                cart = new Cart {
                    CustomerId = addToCartDto.CustomerId,
                    IsCheckedOut = false,
                    CartItems = new List<CartItem>()
                };
                unitOfWork.CartRepository.Add(cart); 
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == addToCartDto.ProductId);
            if (existingItem != null)
            {
                if (existingItem.Quantity + addToCartDto.Quantity > product.StockQuantity)
                    return new ApiResponse<CartResponse>(400, "Exceeds available stock.");

                existingItem.Quantity += addToCartDto.Quantity;
                existingItem.TotalPrice = (existingItem.UnitPrice - existingItem.Discount) * existingItem.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var discount = product.DiscountPercentage > 0 ? product.Price * product.DiscountPercentage / 100 : 0;
                var newItem = new CartItem {
                    ProductId = product.Id,
                    Quantity = addToCartDto.Quantity,
                    UnitPrice = product.Price,
                    Discount = discount,
                    TotalPrice = (product.Price - discount) * addToCartDto.Quantity
                };
                cart.CartItems.Add(newItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();

            var updatedCart = await unitOfWork.CartRepository.GetByIdWithItemsAsync(cart.Id);
            return new ApiResponse<CartResponse>(200, MapCartToDto(updatedCart!));
        }
        catch (Exception ex)
        {
            return new ApiResponse<CartResponse>(500, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CartResponse>> UpdateCartItemAsync(UpdateCartItemRequest updateDto)
    {
        try
        {
            var cart = await unitOfWork.CartRepository.GetActiveCartByCustomerIdAsync(updateDto.CustomerId, trackChanges: true);
            if (cart == null) return new ApiResponse<CartResponse>(404, "Cart not found.");

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == updateDto.CartItemId);
            if (item == null) return new ApiResponse<CartResponse>(404, "Item not found.");

            if (updateDto.Quantity > item.Product.StockQuantity)
                return new ApiResponse<CartResponse>(400, "Not enough stock.");

            item.Quantity = updateDto.Quantity;
            item.TotalPrice = (item.UnitPrice - item.Discount) * item.Quantity;
            item.UpdatedAt = DateTime.UtcNow;
            cart.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();

            return new ApiResponse<CartResponse>(200, MapCartToDto(cart));
        }
        catch (Exception ex)
        {
            return new ApiResponse<CartResponse>(500, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CartResponse>> RemoveCartItemAsync(RemoveCartItemDTO removeCartItemDto)
    {
        try
        {
            var cart = await unitOfWork.CartRepository.GetActiveCartByCustomerIdAsync(removeCartItemDto.CustomerId, trackChanges: true);
            if (cart == null) return new ApiResponse<CartResponse>(404, "Cart not found.");

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == removeCartItemDto.CartItemId);
            if (item == null) return new ApiResponse<CartResponse>(404, "Item not found.");

            unitOfWork.CartItemRepository.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();

            var updatedCart = await unitOfWork.CartRepository.GetByIdWithItemsAsync(cart.Id);
            return new ApiResponse<CartResponse>(200, MapCartToDto(updatedCart ?? new Cart()));
        }
        catch (Exception ex)
        {
            return new ApiResponse<CartResponse>(500, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ConfirmationResponse>> ClearCartAsync(int customerId)
    {
        try
        {
            var cart = await unitOfWork.CartRepository.GetActiveCartByCustomerIdAsync(customerId, trackChanges: true);
            if (cart == null) return new ApiResponse<ConfirmationResponse>(404, "Cart not found.");

            if (cart.CartItems.Any())
            {
                unitOfWork.CartItemRepository.RemoveRange(cart.CartItems);
                cart.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.SaveChangesAsync();
            }

            return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse { Message = "Cart cleared." });
        }
        catch (Exception ex)
        {
            return new ApiResponse<ConfirmationResponse>(500, $"Error: {ex.Message}");
        }
    }

    private CartResponse CreateEmptyCartResponse(int customerId) => new() {
        CustomerId = customerId,
        CartItems = new List<CartItemResponse>(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private CartResponse MapCartToDto(Cart cart)
    {
        var response = mapper.Map(cart);
        response.TotalBasePrice = cart.CartItems.Sum(i => i.UnitPrice * i.Quantity);
        response.TotalDiscount = cart.CartItems.Sum(i => i.Discount * i.Quantity);
        response.TotalAmount = cart.CartItems.Sum(i => i.TotalPrice);
        
        if (response.CartItems == null) return response;
        
        foreach(var item in response.CartItems)
        {
            var sourceItem = cart.CartItems.First(i => i.Id == item.Id || i.ProductId == item.ProductId);
            item.ProductName = sourceItem.Product?.Name;
        }

        return response;
    }
}