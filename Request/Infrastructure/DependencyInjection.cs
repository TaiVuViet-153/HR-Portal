using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Request.Application.Interfaces;
using Request.Application.Repositories;
using Request.Domain.Repositories;
using Request.Infrastructure.Persistence;
using Request.Infrastructure.Repositories;
using Request.Infrastructure.Services.Email;

namespace Request.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<GraphEmailOptions>(configuration.GetSection("GraphEmail"));

        services.AddScoped<ILeaveRepository, LeaveRepository>();
        services.AddScoped<IBalanceRepository, LeaveRepository>();
        services.AddScoped<IRequestRepository, LeaveRepository>();
        services.AddScoped<IEmailSender, GraphEmailSender>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RequestDbContext>());

        return services;
    }
}