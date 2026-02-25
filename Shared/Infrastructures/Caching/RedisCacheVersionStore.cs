using StackExchange.Redis;
using Shared.Abstractions.Caching;

namespace Shared.Infrastructures.Caching;

public class RedisCacheVersionStore : ICacheVersionStore
{
    private readonly IDatabase _database;
    private const string VersionKeyPrefix = "version:";

    public RedisCacheVersionStore(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<long> GetVersionAsync(string prefix)
    {
        var key = GetVersionKey(prefix);
        var value = await _database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
        {
            await _database.StringSetAsync(key, 1);
            return 1;
        }

        return (long)value;
    }

    public async Task<long> IncrementVersionAsync(string prefix)
    {
        var key = GetVersionKey(prefix);
        return await _database.StringIncrementAsync(key);
    }

    private static string GetVersionKey(string prefix) => $"{VersionKeyPrefix}{prefix}";
}