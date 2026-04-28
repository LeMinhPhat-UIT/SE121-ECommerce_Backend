using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ProductDTOs;
using ECommerceApp.Entites;
using ECommerceApp.Mappings.Products;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductMapper _mapper;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IProductMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProductResponse>> CreateProductAsync(ProductCreateRequest productDto)
        {
            try
            {
                if (await _productRepository.ExistsByNameAsync(productDto.Name))
                {
                    return new ApiResponse<ProductResponse>(400, "Product name already exists.");
                }

                if (!await _categoryRepository.ExistsByIdAsync(productDto.CategoryId))
                {
                    return new ApiResponse<ProductResponse>(400, "Specified category does not exist.");
                }

                var product = _mapper.Map(productDto);
                product.IsAvailable = true;

                await _productRepository.AddAsync(product);

                return new ApiResponse<ProductResponse>(200, _mapper.Map(product));
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
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return new ApiResponse<ProductResponse>(404, "Product not found.");
                }

                return new ApiResponse<ProductResponse>(200, _mapper.Map(product));
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
                var product = await _productRepository.GetByIdAsync(productDto.Id);
                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Product not found.");
                }

                if (await _productRepository.ExistsByNameAsync(productDto.Name, productDto.Id))
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Another product with the same name already exists.");
                }

                if (!await _categoryRepository.ExistsByIdAsync(productDto.CategoryId))
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

                await _productRepository.UpdateAsync(product);

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
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Product not found.");
                }

                product.IsAvailable = false;
                await _productRepository.UpdateAsync(product);

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
                var products = await _productRepository.GetAllAsync();

                var productList = products.Select(_mapper.Map).ToPagedResult(paginationRequest);

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
                var products = await _productRepository.GetByCategoryAsync(categoryId);

                if (products.Count == 0)
                {
                    return new ApiResponse<PagedResult<ProductResponse>>(404, "Products not found.");
                }

                var productList = products.Select(_mapper.Map).ToPagedResult(paginationRequest);

                return new ApiResponse<PagedResult<ProductResponse>>(200, productList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<ProductResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdateProductStatusAsync(ProductStatusUpdateRequest productStatusUpdateDTO)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productStatusUpdateDTO.ProductId);

                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Product not found.");
                }

                product.IsAvailable = productStatusUpdateDTO.IsAvailable;
                await _productRepository.UpdateAsync(product);

                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Product with Id {productStatusUpdateDTO.ProductId} Status Updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

    }
}