using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ProductDTOs;

namespace ECommerceApp.Services.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<ProductResponse>> CreateProductAsync(ProductCreateRequest productDto);
        Task<ApiResponse<ProductResponse>> GetProductByIdAsync(int id);
        Task<ApiResponse<ConfirmationResponse>> UpdateProductAsync(ProductUpdateRequest productDto);
        Task<ApiResponse<ConfirmationResponse>> DeleteProductAsync(int id);
        Task<ApiResponse<PagedResult<ProductResponse>>> GetAllProductsAsync(PaginationRequest paginationRequest);
        Task<ApiResponse<PagedResult<ProductResponse>>> GetAllProductsByCategoryAsync(int categoryId, PaginationRequest paginationRequest);
        Task<ApiResponse<ConfirmationResponse>> UpdateProductStatusAsync(ProductStatusUpdateRequest productStatusUpdateDTO);
    }
}