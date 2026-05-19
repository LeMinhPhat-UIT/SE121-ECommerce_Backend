namespace ECommerceApp.Services.Caching
{
    public static class CatalogCacheKeys
    {
        public const string ProductVersion = "catalog:products:version";
        public const string CategoryVersion = "catalog:categories:version";

        public static string ProductById(int productId, string version)
        {
            return $"catalog:products:{version}:id:{productId}";
        }

        public static string CategoryById(int categoryId, string version)
        {
            return $"catalog:categories:{version}:id:{categoryId}";
        }
    }
}
