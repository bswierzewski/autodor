using System.Reflection;
using Autodor.Modules.Contractors.Application.Module;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application;
using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Autodor.Modules.Contractors.Infrastructure.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services
            .AddMigrationService<ContractorsDbContext>()
            .AddAuditableEntityInterceptor()
            .AddDomainEventDispatchInterceptor();
            
        services.AddDbContext<ContractorsDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ContractorsConnection"))
                   .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        services.AddScoped<IContractorsWriteDbContext>(provider => provider.GetRequiredService<ContractorsDbContext>());
        services.AddScoped<IContractorsReadDbContext>(provider => provider.GetRequiredService<ContractorsDbContext>());

        return services;
    }
}