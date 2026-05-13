using ECommerceApp.Commons;
using ECommerceApp.DTOs.ProductDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Products;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.Implements
{
    public class ProductService(
        IUnitOfWork unitOfWork,
        IProductMapper mapper)
        : IProductService
    {
        public async Task<ApiResponse<ProductResponse>> CreateProductAsync(ProductCreateRequest productDto)
        {
            try
            {
                if (await unitOfWork.ProductRepository.ExistsByNameAsync(productDto.Name))
                {
                    return new ApiResponse<ProductResponse>(400, "Product name already exists.");
                }

                if (!await unitOfWork.CategoryRepository.ExistsByIdAsync(productDto.CategoryId))
                {
                    return new ApiResponse<ProductResponse>(400, "Specified category does not exist.");
                }

                var product = mapper.Map(productDto);
                product.IsAvailable = true;

                
                unitOfWork.ProductRepository.Add(product);
                await unitOfWork.SaveChangesAsync();
                    
                return new ApiResponse<ProductResponse>(200, mapper.Map(product));
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductResponse>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return new ApiResponse<ProductResponse>(404, "Product not found.");
                }

                return new ApiResponse<ProductResponse>(200, mapper.Map(product));
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdateProductAsync(ProductUpdateRequest productDto)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(productDto.Id, true);
                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Product not found.");
                }

                if (await unitOfWork.ProductRepository.ExistsByNameAsync(productDto.Name, productDto.Id))
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Another product with the same name already exists.");
                }

                if (!await unitOfWork.CategoryRepository.ExistsByIdAsync(productDto.CategoryId))
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Specified category does not exist.");
                }

                product.Name = productDto.Name;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.StockQuantity = productDto.StockQuantity;
                product.ImageUrl = productDto.ImageUrl;
                product.DiscountPercentage = productDto.DiscountPercentage;
                product.CategoryId = productDto.CategoryId;

                await unitOfWork.SaveChangesAsync();
                
                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Product with Id {productDto.Id} updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdIncludingUnavailableAsync(id, trackChanges: true);

                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Product not found.");
                }

                product.IsAvailable = false;

                await unitOfWork.SaveChangesAsync();
                
                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Product with Id {id} deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<ProductResponse>>> GetAllProductsAsync(PaginationRequest paginationRequest)
        {
            try
            {
                var safeRequest = new PaginationRequest
                {
                    PageIndex = paginationRequest?.PageIndex > 0 ? paginationRequest.PageIndex : 1,
                    PageSize = paginationRequest?.PageSize > 0 ? paginationRequest.PageSize : 10
                };

                var productQuery = unitOfWork.ProductRepository.QueryAllAvailable();
                var totalCount = await productQuery.CountAsync();
                var products = await productQuery
                    .Skip((safeRequest.PageIndex - 1) * safeRequest.PageSize)
                    .Take(safeRequest.PageSize)
                    .ToListAsync();

                var productList = new PagedResult<ProductResponse>(products.Select(mapper.Map), safeRequest, totalCount);

                return new ApiResponse<PagedResult<ProductResponse>>(200, productList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<ProductResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<ProductResponse>>> GetAllProductsByCategoryAsync(int categoryId, PaginationRequest paginationRequest)
        {
            try
            {
                var safeRequest = new PaginationRequest
                {
                    PageIndex = paginationRequest?.PageIndex > 0 ? paginationRequest.PageIndex : 1,
                    PageSize = paginationRequest?.PageSize > 0 ? paginationRequest.PageSize : 10
                };

                var productQuery = unitOfWork.ProductRepository.QueryByCategory(categoryId);
                var totalCount = await productQuery.CountAsync();

                if (totalCount == 0)
                {
                    return new ApiResponse<PagedResult<ProductResponse>>(404, "Products not found.");
                }

                var products = await productQuery
                    .Skip((safeRequest.PageIndex - 1) * safeRequest.PageSize)
                    .Take(safeRequest.PageSize)
                    .ToListAsync();

                var productList = new PagedResult<ProductResponse>(products.Select(mapper.Map), safeRequest, totalCount);

                return new ApiResponse<PagedResult<ProductResponse>>(200, productList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<ProductResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdateProductStatusAsync(ProductStatusUpdateRequest productStatusUpdateDto)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(productStatusUpdateDto.ProductId, true);

                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Product not found.");
                }

                product.IsAvailable = productStatusUpdateDto.IsAvailable;

                await unitOfWork.SaveChangesAsync();
                
                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Product with Id {productStatusUpdateDto.ProductId} Status Updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

    }
}