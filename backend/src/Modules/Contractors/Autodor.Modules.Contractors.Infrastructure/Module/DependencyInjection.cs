using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Contractors.Infrastructure.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
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