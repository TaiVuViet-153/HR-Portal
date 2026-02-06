using Microsoft.Extensions.DependencyInjection;
using Employee.Domain.Repositories;
using Employee.Infrastructure.Repositories;
using Employee.Application.Repositories.Queries;
using Employee.Application.Interfaces;
using Employee.Infrastructure.Services.Email;
using Microsoft.Extensions.Configuration;

namespace Employee.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<GraphEmailOptions>(configuration.GetSection("GraphEmail"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserQueriesRepository, UserRepository>();
        services.AddScoped<IEmailSender, GraphEmailSender>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();

        return services;
    }
}