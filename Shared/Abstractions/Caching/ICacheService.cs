namespace Shared.Abstractions.Caching;

/// <summary>
/// Interface for distributed cache operations with version-based invalidation.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from cache, or creates it using the factory if not found.
    /// Automatically includes version in cache key for invalidation support.
    /// </summary>
    /// <typeparam name="T">Type of cached value</typeparam>
    /// <param name="prefix">Cache key prefix for versioning</param>
    /// <param name="key">Unique cache key</param>
    /// <param name="factory">Factory to create value if not cached</param>
    /// <param name="expiration">Optional expiration time</param>
    /// <returns>Cached or newly created value</returns>
    Task<T?> GetOrCreateAsync<T>(
        string prefix,
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null);

    /// <summary>
    /// Gets a value from cache.
    /// </summary>
    Task<T?> GetAsync<T>(string prefix, string key);

    /// <summary>
    /// Sets a value in cache.
    /// </summary>
    Task SetAsync<T>(string prefix, string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a specific cache entry.
    /// </summary>
    Task RemoveAsync(string prefix, string key);

    /// <summary>
    /// Invalidates all cache entries with the given prefix by incrementing version.
    /// </summary>
    /// <param name="prefix">Cache key prefix to invalidate</param>
    Task InvalidateByPrefixAsync(string prefix);
}