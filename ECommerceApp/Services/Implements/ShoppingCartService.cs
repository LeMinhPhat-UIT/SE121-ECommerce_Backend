using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ShoppingCartDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Carts;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartMapper _mapper;

        public ShoppingCartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IProductRepository productRepository, ICartMapper mapper)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CartResponse>> GetCartByCustomerIdAsync(int customerId)
        {
            try
            {
                var cart = await _cartRepository.GetActiveCartByCustomerIdAsync(customerId);

                if (cart == null)
                {
                    var emptyCartDTO = new CartResponse
                    {
                        CustomerId = customerId,
                        IsCheckedOut = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItemResponse>(),
                        TotalBasePrice = 0,
                        TotalDiscount = 0,
                        TotalAmount = 0
                    };

                    return new ApiResponse<CartResponse>(200, emptyCartDTO);
                }

                var cartDTO = MapCartToDTO(cart);
                return new ApiResponse<CartResponse>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CartResponse>> AddToCartAsync(AddToCartRequest addToCartDTO)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(addToCartDTO.ProductId);
                if (product == null)
                {
                    return new ApiResponse<CartResponse>(404, "Product not found.");
                }

                if (addToCartDTO.Quantity > product.StockQuantity)
                {
                    return new ApiResponse<CartResponse>(400, $"Only {product.StockQuantity} units of {product.Name} are available.");
                }

                var cart = await _cartRepository.GetActiveCartByCustomerIdAsync(addToCartDTO.CustomerId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        CustomerId = addToCartDTO.CustomerId,
                        IsCheckedOut = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItem>()
                    };

                    await _cartRepository.AddAsync(cart);
                }

                var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == addToCartDTO.ProductId);
                if (existingCartItem != null)
                {
                    if (existingCartItem.Quantity + addToCartDTO.Quantity > product.StockQuantity)
                    {
                        return new ApiResponse<CartResponse>(400, $"Adding {addToCartDTO.Quantity} exceeds available stock.");
                    }

                    existingCartItem.Quantity += addToCartDTO.Quantity;
                    existingCartItem.TotalPrice = (existingCartItem.UnitPrice - existingCartItem.Discount) * existingCartItem.Quantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;

                    await _cartItemRepository.UpdateAsync(existingCartItem);
                }
                else
                {
                    var discount = product.DiscountPercentage > 0 ? product.Price * product.DiscountPercentage / 100 : 0;

                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = product.Id,
                        Quantity = addToCartDTO.Quantity,
                        UnitPrice = product.Price,
                        Discount = discount,
                        TotalPrice = (product.Price - discount) * addToCartDTO.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _cartItemRepository.AddAsync(cartItem);
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateAsync(cart);

                cart = await _cartRepository.GetByIdWithItemsAsync(cart.Id) ?? new Cart();

                var cartDTO = MapCartToDTO(cart);
                return new ApiResponse<CartResponse>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CartResponse>> UpdateCartItemAsync(UpdateCartItemRequest updateCartItemDTO)
        {
            try
            {
                var cart = await _cartRepository.GetActiveCartByCustomerIdAsync(updateCartItemDTO.CustomerId);

                if (cart == null)
                {
                    return new ApiResponse<CartResponse>(404, "Active cart not found.");
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == updateCartItemDTO.CartItemId);
                if (cartItem == null)
                {
                    return new ApiResponse<CartResponse>(404, "Cart item not found.");
                }

                if (updateCartItemDTO.Quantity > cartItem.Product.StockQuantity)
                {
                    return new ApiResponse<CartResponse>(400, $"Only {cartItem.Product.StockQuantity} units of {cartItem.Product.Name} are available.");
                }

                cartItem.Quantity = updateCartItemDTO.Quantity;
                cartItem.TotalPrice = (cartItem.UnitPrice - cartItem.Discount) * cartItem.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;

                await _cartItemRepository.UpdateAsync(cartItem);

                cart.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateAsync(cart);

                cart = await _cartRepository.GetByIdWithItemsAsync(cart.Id) ?? new Cart();

                var cartDTO = MapCartToDTO(cart);
                return new ApiResponse<CartResponse>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CartResponse>> RemoveCartItemAsync(RemoveCartItemDTO removeCartItemDTO)
        {
            try
            {
                var cart = await _cartRepository.GetActiveCartByCustomerIdAsync(removeCartItemDTO.CustomerId);

                if (cart == null)
                {
                    return new ApiResponse<CartResponse>(404, "Active cart not found.");
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == removeCartItemDTO.CartItemId);
                if (cartItem == null)
                {
                    return new ApiResponse<CartResponse>(404, "Cart item not found.");
                }

                await _cartItemRepository.RemoveAsync(cartItem);
                cart.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateAsync(cart);

                cart = await _cartRepository.GetByIdWithItemsAsync(cart.Id) ?? new Cart();

                // Map the updated cart to the DTO.
                var cartDTO = MapCartToDTO(cart);
                return new ApiResponse<CartResponse>(200, cartDTO);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> ClearCartAsync(int customerId)
        {
            try
            {
                var cart = await _cartRepository.GetActiveCartByCustomerIdAsync(customerId);

                if (cart == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Active cart not found.");
                }

                if (cart.CartItems.Any())
                {
                    await _cartItemRepository.RemoveRangeAsync(cart.CartItems);
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _cartRepository.UpdateAsync(cart);
                }

                var confirmation = new ConfirmationResponse
                {
                    Message = "Cart has been cleared successfully."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmation);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        private CartResponse MapCartToDTO(Cart cart)
        {
            var cartItemsDto = cart.CartItems?.Select(ci =>
            {
                var cartItem = _mapper.Map(ci);
                cartItem.ProductName = ci.Product?.Name;
                return cartItem;
            }).ToList() ?? new List<CartItemResponse>();

            decimal totalBasePrice = 0;
            decimal totalDiscount = 0;
            decimal totalAmount = 0;

            foreach (var item in cartItemsDto)
            {
                totalBasePrice += item.UnitPrice * item.Quantity;
                totalDiscount += item.Discount * item.Quantity;
                totalAmount += item.TotalPrice;
            }

            var cartResponse = _mapper.Map(cart);
            cartResponse.CartItems = cartItemsDto;
            cartResponse.TotalBasePrice = totalBasePrice;
            cartResponse.TotalDiscount = totalDiscount;
            cartResponse.TotalAmount = totalAmount;

            return cartResponse;
        }
    }
}