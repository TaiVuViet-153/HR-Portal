using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Caching;
using Shared.Infrastructures.Caching;
using StackExchange.Redis;

namespace Shared.Infrastructures;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedLibraries(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddRedisCache(configuration);

        return services;
    }

    private static IServiceCollection AddRedisCache(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var redisSection = configuration.GetSection("Redis");

        if (redisSection != null)
        {
            services.Configure<RedisOptions>(redisSection);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "HRM:";
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connectionString = configuration.GetConnectionString("Redis")!;
                var options = ConfigurationOptions.Parse(connectionString);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options);
            });

            services.AddSingleton<ICacheVersionStore, RedisCacheVersionStore>();
            services.AddSingleton<ICacheService, RedisCacheService>();
        }

        return services;
    }
}