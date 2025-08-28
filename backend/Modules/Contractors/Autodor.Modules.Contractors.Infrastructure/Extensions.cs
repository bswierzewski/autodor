using System.Reflection;
using Autodor.Modules.Contractors.Domain.Abstractions;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Modules.Contractors.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure;

namespace Autodor.Modules.Contractors.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddContractors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));        

        services.AddDbContext<ContractorsDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ContractorsConnection"));

            // This adds AuditableEntityInterceptor and DispatchDomainEventsInterceptor
            options.AddInterceptors(serviceProvider);
        });

        // 3. Register repositories and UnitOfWork
        services.AddRepositories<ContractorsDbContext>();

        // Rejestracja serwisu do uruchamiania migracji
        services.AddHostedService<ContractorsMigrationService>();

        // Rejestracja Repository + UnitOfWork
        services.AddScoped<IContractorRepository, ContractorRepository>();

        return services;
    }
}