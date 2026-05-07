using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ProductDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> CreateProduct([FromBody] ProductCreateRequest productDto)
        {
            _logger.LogInformation("CreateProduct request received for Name={Name} CategoryId={CategoryId}", productDto.Name, productDto.CategoryId);
            var response = await _productService.CreateProductAsync(productDto);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("CreateProduct failed for Name={Name} with StatusCode={StatusCode}", productDto.Name, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("CreateProduct succeeded for Name={Name}", productDto.Name);
            return Ok(response);
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProductById(int id)
        {
            _logger.LogInformation("GetProductById request received for ProductId={ProductId}", id);
            var response = await _productService.GetProductByIdAsync(id);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("GetProductById failed for ProductId={ProductId} with StatusCode={StatusCode}", id, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("GetProductById succeeded for ProductId={ProductId}", id);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateProduct")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateProduct([FromBody] ProductUpdateRequest productDto)
        {
            _logger.LogInformation("UpdateProduct request received for ProductId={ProductId}", productDto.Id);
            var response = await _productService.UpdateProductAsync(productDto);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("UpdateProduct failed for ProductId={ProductId} with StatusCode={StatusCode}", productDto.Id, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("UpdateProduct succeeded for ProductId={ProductId}", productDto.Id);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteProduct(int id)
        {
            _logger.LogInformation("DeleteProduct request received for ProductId={ProductId}", id);
            var response = await _productService.DeleteProductAsync(id);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("DeleteProduct failed for ProductId={ProductId} with StatusCode={StatusCode}", id, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("DeleteProduct succeeded for ProductId={ProductId}", id);
            return Ok(response);
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductResponse>>>> GetAllProducts([FromQuery] PaginationRequest paginationRequest)
        {
            _logger.LogInformation("GetAllProducts request received with PageNumber={PageNumber} PageSize={PageSize}", paginationRequest.PageIndex, paginationRequest.PageSize);
            var response = await _productService.GetAllProductsAsync(paginationRequest);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("GetAllProducts failed with StatusCode={StatusCode}", response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("GetAllProducts succeeded");
            return Ok(response);
        }

        [HttpGet("GetAllProductsByCategory/{categoryId}")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductResponse>>>> GetAllProductsByCategory(int categoryId, [FromQuery] PaginationRequest paginationRequest)
        {
            _logger.LogInformation("GetAllProductsByCategory request received for CategoryId={CategoryId}", categoryId);
            var response = await _productService.GetAllProductsByCategoryAsync(categoryId, paginationRequest);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("GetAllProductsByCategory failed for CategoryId={CategoryId} with StatusCode={StatusCode}", categoryId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("GetAllProductsByCategory succeeded for CategoryId={CategoryId}", categoryId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateProductStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateProductStatus(ProductStatusUpdateRequest productStatusUpdateDTO)
        {
            _logger.LogInformation("UpdateProductStatus request received for ProductId={ProductId} IsAvailable={IsAvailable}", productStatusUpdateDTO.ProductId, productStatusUpdateDTO.IsAvailable);
            var response = await _productService.UpdateProductStatusAsync(productStatusUpdateDTO);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("UpdateProductStatus failed for ProductId={ProductId} with StatusCode={StatusCode}", productStatusUpdateDTO.ProductId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("UpdateProductStatus succeeded for ProductId={ProductId}", productStatusUpdateDTO.ProductId);
            return Ok(response);
        }
    }
}