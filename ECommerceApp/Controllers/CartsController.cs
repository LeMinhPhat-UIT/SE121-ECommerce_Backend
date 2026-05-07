using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ShoppingCartDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger<CartsController> _logger;

        public CartsController(IShoppingCartService shoppingCartService, ILogger<CartsController> logger)
        {
            _shoppingCartService = shoppingCartService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("GetCart/{customerId}")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> GetCartByCustomerId(int customerId)
        {
            _logger.LogInformation("GetCart request received for CustomerId={CustomerId}", customerId);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                _logger.LogWarning("GetCart forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, customerId);
                return Forbid();
            }

            var response = await _shoppingCartService.GetCartByCustomerIdAsync(customerId);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("GetCart failed for CustomerId={CustomerId} with StatusCode={StatusCode}", customerId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("GetCart succeeded for CustomerId={CustomerId}", customerId);
            return Ok(response);
        }

        // Adds an item to the customer's cart.
        [Authorize]
        [HttpPost("AddToCart")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> AddToCart([FromBody] AddToCartRequest addToCartDTO)
        {
            _logger.LogInformation("AddToCart request received for CustomerId={CustomerId} ProductId={ProductId} Quantity={Quantity}", addToCartDTO.CustomerId, addToCartDTO.ProductId, addToCartDTO.Quantity);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != addToCartDTO.CustomerId)
            {
                _logger.LogWarning("AddToCart forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, addToCartDTO.CustomerId);
                return Forbid();
            }

            var response = await _shoppingCartService.AddToCartAsync(addToCartDTO);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("AddToCart failed for CustomerId={CustomerId} ProductId={ProductId} with StatusCode={StatusCode}", addToCartDTO.CustomerId, addToCartDTO.ProductId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("AddToCart succeeded for CustomerId={CustomerId} ProductId={ProductId}", addToCartDTO.CustomerId, addToCartDTO.ProductId);
            return Ok(response);
        }

        // Updates the quantity of an existing cart item.
        [Authorize]
        [HttpPut("UpdateCartItem")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> UpdateCartItem([FromBody] UpdateCartItemRequest updateCartItemDTO)
        {
            _logger.LogInformation("UpdateCartItem request received for CustomerId={CustomerId} CartItemId={CartItemId} Quantity={Quantity}", updateCartItemDTO.CustomerId, updateCartItemDTO.CartItemId, updateCartItemDTO.Quantity);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != updateCartItemDTO.CustomerId)
            {
                _logger.LogWarning("UpdateCartItem forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, updateCartItemDTO.CustomerId);
                return Forbid();
            }

            var response = await _shoppingCartService.UpdateCartItemAsync(updateCartItemDTO);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("UpdateCartItem failed for CustomerId={CustomerId} CartItemId={CartItemId} with StatusCode={StatusCode}", updateCartItemDTO.CustomerId, updateCartItemDTO.CartItemId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("UpdateCartItem succeeded for CustomerId={CustomerId} CartItemId={CartItemId}", updateCartItemDTO.CustomerId, updateCartItemDTO.CartItemId);
            return Ok(response);
        }

        // Removes a specific item from the cart.
        [Authorize]
        [HttpDelete("RemoveCartItem")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> RemoveCartItem([FromBody] RemoveCartItemDTO removeCartItemDTO)
        {
            _logger.LogInformation("RemoveCartItem request received for CustomerId={CustomerId} CartItemId={CartItemId}", removeCartItemDTO.CustomerId, removeCartItemDTO.CartItemId);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != removeCartItemDTO.CustomerId)
            {
                _logger.LogWarning("RemoveCartItem forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, removeCartItemDTO.CustomerId);
                return Forbid();
            }

            var response = await _shoppingCartService.RemoveCartItemAsync(removeCartItemDTO);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("RemoveCartItem failed for CustomerId={CustomerId} CartItemId={CartItemId} with StatusCode={StatusCode}", removeCartItemDTO.CustomerId, removeCartItemDTO.CartItemId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("RemoveCartItem succeeded for CustomerId={CustomerId} CartItemId={CartItemId}", removeCartItemDTO.CustomerId, removeCartItemDTO.CartItemId);
            return Ok(response);
        }

        // Clears all items from the customer's active cart.
        [Authorize]
        [HttpDelete("ClearCart")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> ClearCart([FromQuery] int customerId)
        {
            _logger.LogInformation("ClearCart request received for CustomerId={CustomerId}", customerId);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                _logger.LogWarning("ClearCart forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, customerId);
                return Forbid();
            }

            var response = await _shoppingCartService.ClearCartAsync(customerId);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("ClearCart failed for CustomerId={CustomerId} with StatusCode={StatusCode}", customerId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("ClearCart succeeded for CustomerId={CustomerId}", customerId);
            return Ok(response);
        }
    }
}