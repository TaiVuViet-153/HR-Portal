using System.Reflection;

namespace Shared.Abstractions.Caching;

/// <summary>
/// Utility class for building cache keys from objects.
/// Can be reused across multiple services.
/// </summary>
public static class CacheKeyBuilder
{
    /// <summary>
    /// Builds a cache key by combining all public properties of the query object.
    /// </summary>
    /// <param name="query">The query object containing filter/paging parameters</param>
    /// <returns>A unique cache key string</returns>
    public static string Build(object? query)
    {
        if (query == null)
            return "default";

        var parts = query
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(p => p.Name)
            .Select(p => FormatPropertyValue(p, query));

        return string.Join("|", parts);
    }

    /// <summary>
    /// Builds a cache key with a custom suffix.
    /// </summary>
    /// <param name="query">The query object</param>
    /// <param name="suffix">Additional suffix to append</param>
    /// <returns>A unique cache key string</returns>
    public static string Build(object? query, string suffix)
    {
        var baseKey = Build(query);
        return string.IsNullOrEmpty(suffix) ? baseKey : $"{baseKey}:{suffix}";
    }

    private static string FormatPropertyValue(PropertyInfo prop, object obj)
    {
        var value = prop.GetValue(obj);

        return value switch
        {
            null => $"{prop.Name}=null",
            DateTime dt => $"{prop.Name}={dt:yyyyMMddHHmmss}",
            DateOnly d => $"{prop.Name}={d:yyyyMMdd}",
            TimeOnly t => $"{prop.Name}={t:HHmmss}",
            _ => $"{prop.Name}={value}"
        };
    }
}
