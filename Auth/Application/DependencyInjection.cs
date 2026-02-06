using Auth.Application.Services;
using Auth.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}