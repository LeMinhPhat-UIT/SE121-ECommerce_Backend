namespace ECommerceApp.Services.Caching
{
    public class CacheOptions
    {
        public const string SectionName = "Cache";

        public bool Enabled { get; set; } = true;

        public int DefaultExpirationMinutes { get; set; } = 10;

        public int CatalogExpirationMinutes { get; set; } = 15;
    }
}
