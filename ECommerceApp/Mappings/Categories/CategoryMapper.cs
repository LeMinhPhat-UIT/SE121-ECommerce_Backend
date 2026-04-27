using ECommerceApp.DTOs.CategoryDTOs;
using ECommerceApp.Entities;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Categories
{
    public interface ICategoryMapper
    {
        CategoryResponse Map(Category source);

        Category Map(CategoryCreateRequest source);

        void Map(CategoryUpdateRequest source, Category destination);
    }

    [Mapper]
    public partial class CategoryMapper : ICategoryMapper
    {
        public partial CategoryResponse Map(Category source);

        public partial Category Map(CategoryCreateRequest source);

        public partial void Map(CategoryUpdateRequest source, Category destination);
    }
}