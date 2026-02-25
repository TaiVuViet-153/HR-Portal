namespace Shared.Abstractions.Caching;

/// <summary>
/// Interface for managing cache versions.
/// Version-based invalidation: when data changes, increment version to invalidate all related caches.
/// </summary>
public interface ICacheVersionStore
{
    /// <summary>
    /// Gets the current version for a cache key prefix.
    /// </summary>
    /// <param name="prefix">The cache key prefix (e.g., "LeaveRequests")</param>
    /// <returns>Current version number</returns>
    Task<long> GetVersionAsync(string prefix);

    /// <summary>
    /// Increments the version for a cache key prefix, effectively invalidating all caches with that prefix.
    /// </summary>
    /// <param name="prefix">The cache key prefix to invalidate</param>
    /// <returns>New version number</returns>
    Task<long> IncrementVersionAsync(string prefix);
}