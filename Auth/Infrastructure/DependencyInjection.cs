using Auth.Domain.Repositories;
using Auth.Domain.Services;
using Auth.Infrastructure.Repositories;
using Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}