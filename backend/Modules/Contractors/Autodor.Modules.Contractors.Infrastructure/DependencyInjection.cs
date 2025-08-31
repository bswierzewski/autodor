using System.Reflection;
using Autodor.Modules.Contractors.Application;
using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure;

namespace Autodor.Modules.Contractors.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddContractors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));        

        services.AddDbContext<ContractorsDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ContractorsConnection"));

            // This adds AuditableEntityInterceptor and DispatchDomainEventsInterceptor
            options.AddInterceptors(serviceProvider);
        });

        // DbContext
        services.AddModuleContext<ContractorsDbContext>(module =>
        {
            module.AddMigrations();
            module.AddRepository<Contractor>();
        });

        return services;
    }
}