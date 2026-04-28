using ECommerceApp.Commons;
using ECommerceApp.DTOs.CategoryDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Categories;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, ICategoryMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CategoryResponse>> CreateCategoryAsync(CategoryCreateRequest categoryDto)
        {
            try
            {
                if (await _categoryRepository.ExistsByNameAsync(categoryDto.Name))
                {
                    return new ApiResponse<CategoryResponse>(400, "Category name already exists.");
                }

                var category = _mapper.Map(categoryDto);
                category.IsActive = true;

                await _categoryRepository.AddAsync(category);

                return new ApiResponse<CategoryResponse>(200, _mapper.Map(category));
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CategoryResponse>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);

                if (category == null)
                {
                    return new ApiResponse<CategoryResponse>(404, "Category not found.");
                }

                return new ApiResponse<CategoryResponse>(200, _mapper.Map(category));
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdateCategoryAsync(CategoryUpdateRequest categoryDto)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryDto.Id);
                if (category == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Category not found.");
                }

                if (await _categoryRepository.ExistsByNameAsync(categoryDto.Name, categoryDto.Id))
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Another category with the same name already exists.");
                }

                category.Name = categoryDto.Name;
                category.Description = categoryDto.Description;

                await _categoryRepository.UpdateAsync(category);

                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Category with Id {categoryDto.Id} updated successfully."
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);

                if (category == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Category not found.");
                }

                category.IsActive = false;
                await _categoryRepository.UpdateAsync(category);

                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Category with Id {id} deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<CategoryResponse>>> GetAllCategoriesAsync(PaginationRequest paginationRequest)
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();

                var categoryList = categories.Select(_mapper.Map).ToPagedResult(paginationRequest);

                return new ApiResponse<PagedResult<CategoryResponse>>(200, categoryList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<CategoryResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
    }
}