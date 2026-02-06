using Microsoft.Extensions.DependencyInjection;
using Employee.Application.Services;
using Employee.Application.Interfaces;
using Employee.Domain.Repositories;
using Employee.Application.Repositories.Queries;

namespace Employee.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}