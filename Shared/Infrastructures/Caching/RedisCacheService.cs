using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Shared.Abstractions.Caching;

namespace Shared.Infrastructures.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ICacheVersionStore _versionStore;
    private readonly RedisOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IDistributedCache cache,
        ICacheVersionStore versionStore,
        IOptions<RedisOptions> options)
    {
        _cache = cache;
        _versionStore = versionStore;
        _options = options.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T?> GetOrCreateAsync<T>(
        string prefix,
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null)
    {
        var fullKey = await BuildVersionedKeyAsync(prefix, key);

        var cached = await _cache.GetStringAsync(fullKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<T>(cached, _jsonOptions);
        }

        var value = await factory();

        if (value is not null)
        {
            await SetInternalAsync(fullKey, value, expiration);
        }

        return value;
    }

    public async Task<T?> GetAsync<T>(string prefix, string key)
    {
        var fullKey = await BuildVersionedKeyAsync(prefix, key);
        var cached = await _cache.GetStringAsync(fullKey);

        if (string.IsNullOrEmpty(cached))
            return default;

        return JsonSerializer.Deserialize<T>(cached, _jsonOptions);
    }

    public async Task SetAsync<T>(string prefix, string key, T value, TimeSpan? expiration = null)
    {
        var fullKey = await BuildVersionedKeyAsync(prefix, key);
        await SetInternalAsync(fullKey, value, expiration);
    }

    public async Task RemoveAsync(string prefix, string key)
    {
        var fullKey = await BuildVersionedKeyAsync(prefix, key);
        await _cache.RemoveAsync(fullKey);
    }

    public async Task InvalidateByPrefixAsync(string prefix)
    {
        await _versionStore.IncrementVersionAsync(prefix);
    }

    private async Task<string> BuildVersionedKeyAsync(string prefix, string key)
    {
        var version = await _versionStore.GetVersionAsync(prefix);
        return $"{_options.InstanceName}{prefix}:v{version}:{key}";
    }

    private async Task SetInternalAsync<T>(string fullKey, T value, TimeSpan? expiration)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(_options.DefaultExpirationMinutes)
        };

        await _cache.SetStringAsync(fullKey, json, options);
    }
}