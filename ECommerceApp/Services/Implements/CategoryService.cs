using ECommerceApp.Commons;
using ECommerceApp.DTOs.CategoryDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Categories;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements
{
    public class CategoryService(IUnitOfWork unitOfWork, ICategoryMapper mapper)
        : ICategoryService
    {
        public async Task<ApiResponse<CategoryResponse>> CreateCategoryAsync(CategoryCreateRequest categoryDto)
        {
            try
            {
                if (await unitOfWork.CategoryRepository.ExistsByNameAsync(categoryDto.Name))
                {
                    return new ApiResponse<CategoryResponse>(400, "Category name already exists.");
                }

                var category = mapper.Map(categoryDto);
                category.IsActive = true;

                unitOfWork.CategoryRepository.Add(category);
                await unitOfWork.SaveChangesAsync();
                
                return new ApiResponse<CategoryResponse>(200, mapper.Map(category));
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
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);

                return category == null ? new ApiResponse<CategoryResponse>(404, "Category not found.") : new ApiResponse<CategoryResponse>(200, mapper.Map(category));
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
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryDto.Id, true);
                if (category == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Category not found.");
                }

                if (await unitOfWork.CategoryRepository.ExistsByNameAsync(categoryDto.Name, categoryDto.Id))
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Another category with the same name already exists.");
                }

                category.Name = categoryDto.Name;
                category.Description = categoryDto.Description;

                await unitOfWork.SaveChangesAsync();

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
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(id, true);

                if (category == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Category not found.");
                }

                category.IsActive = false;
                await unitOfWork.SaveChangesAsync();

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
                var categories = await unitOfWork.CategoryRepository.GetAllAsync();

                var categoryList = categories.Select(mapper.Map).ToPagedResult(paginationRequest);

                return new ApiResponse<PagedResult<CategoryResponse>>(200, categoryList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResult<CategoryResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
    }
}