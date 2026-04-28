using ECommerceApp.DTOs.ProductDTOs;
using ECommerceApp.Entites;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Products
{
    public interface IProductMapper
    {
        ProductResponse Map(Product source);

        Product Map(ProductCreateRequest source);

        void Map(ProductUpdateRequest source, Product destination);
    }

    [Mapper]
    public partial class ProductMapper : IProductMapper
    {
        public partial ProductResponse Map(Product source);

        public partial Product Map(ProductCreateRequest source);

        public partial void Map(ProductUpdateRequest source, Product destination);
    }
}