using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.CategoryDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateCategory")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> CreateCategory([FromBody] CategoryCreateRequest categoryDto)
        {
            _logger.LogInformation("CreateCategory request received for Name={Name}", categoryDto.Name);
            var response = await _categoryService.CreateCategoryAsync(categoryDto);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("CreateCategory failed for Name={Name} with StatusCode={StatusCode}", categoryDto.Name, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("CreateCategory succeeded for Name={Name}", categoryDto.Name);
            return Ok(response);
        }

        [HttpGet("GetCategoryById/{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetCategoryById(int id)
        {
            _logger.LogInformation("GetCategoryById request received for CategoryId={CategoryId}", id);
            var response = await _categoryService.GetCategoryByIdAsync(id);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("GetCategoryById failed for CategoryId={CategoryId} with StatusCode={StatusCode}", id, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("GetCategoryById succeeded for CategoryId={CategoryId}", id);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateCategory")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateCategory([FromBody] CategoryUpdateRequest categoryDto)
        {
            _logger.LogInformation("UpdateCategory request received for CategoryId={CategoryId}", categoryDto.Id);
            var response = await _categoryService.UpdateCategoryAsync(categoryDto);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("UpdateCategory failed for CategoryId={CategoryId} with StatusCode={StatusCode}", categoryDto.Id, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("UpdateCategory succeeded for CategoryId={CategoryId}", categoryDto.Id);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteCategory(int id)
        {
            _logger.LogInformation("DeleteCategory request received for CategoryId={CategoryId}", id);
            var response = await _categoryService.DeleteCategoryAsync(id);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("DeleteCategory failed for CategoryId={CategoryId} with StatusCode={StatusCode}", id, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("DeleteCategory succeeded for CategoryId={CategoryId}", id);
            return Ok(response);
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoryResponse>>>> GetAllCategories([FromQuery] PaginationRequest paginationRequest)
        {
            _logger.LogInformation("GetAllCategories request received with PageNumber={PageNumber} PageSize={PageSize}", paginationRequest.PageIndex, paginationRequest.PageSize);
            var response = await _categoryService.GetAllCategoriesAsync(paginationRequest);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("GetAllCategories failed with StatusCode={StatusCode}", response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("GetAllCategories succeeded");
            return Ok(response);
        }
    }
}