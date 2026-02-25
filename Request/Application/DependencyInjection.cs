using Microsoft.Extensions.DependencyInjection;
using Request.Application.Services;
using Request.Application.Interfaces;

namespace Request.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IBalanceService, BalanceService>();

        return services;
    }
}