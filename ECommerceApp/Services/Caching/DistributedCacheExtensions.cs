using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace ECommerceApp.Services.Caching
{
    public static class DistributedCacheExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public static async Task<T?> GetJsonAsync<T>(
            this IDistributedCache cache,
            string key,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cachedValue = await cache.GetStringAsync(key, cancellationToken);
                return string.IsNullOrWhiteSpace(cachedValue)
                    ? default
                    : JsonSerializer.Deserialize<T>(cachedValue, JsonOptions);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to read cache key {CacheKey}. Falling back to the database.", key);
                return default;
            }
        }

        public static async Task SetJsonAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            TimeSpan absoluteExpirationRelativeToNow,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(value, JsonOptions);
                await cache.SetStringAsync(
                    key,
                    json,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
                    },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to write cache key {CacheKey}.", key);
            }
        }

        public static async Task<string> GetVersionAsync(
            this IDistributedCache cache,
            string key,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var version = await cache.GetStringAsync(key, cancellationToken);
                if (!string.IsNullOrWhiteSpace(version))
                {
                    return version;
                }

                version = DateTimeOffset.UtcNow.Ticks.ToString();
                await cache.SetStringAsync(key, version, cancellationToken);
                return version;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to read cache version key {CacheKey}. Using a request-local version.", key);
                return "nocache";
            }
        }

        public static async Task RefreshVersionAsync(
            this IDistributedCache cache,
            string key,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await cache.SetStringAsync(key, DateTimeOffset.UtcNow.Ticks.ToString(), cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to refresh cache version key {CacheKey}.", key);
            }
        }
    }
}
