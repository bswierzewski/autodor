using System.Reflection;
using Autodor.Modules.Contractors.Domain.Abstractions;
using Autodor.Modules.Contractors.Infrastructure.Interceptors;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddContractors(this IServiceCollection services, IConfiguration configuration)
    {
        // Rejestracja MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // Rejestracja interceptorów - EF Core automatycznie je wykryje
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<DispatchDomainEventsInterceptor>();
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<ContractorsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ContractorsConnection")));

        // Rejestracja Repository + UnitOfWork
        services.AddScoped<IContractorRepository, ContractorRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}