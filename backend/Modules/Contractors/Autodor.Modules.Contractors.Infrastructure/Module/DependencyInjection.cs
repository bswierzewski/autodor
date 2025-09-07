using System.Reflection;
using Autodor.Modules.Contractors.Application.Module;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Infrastructure;

namespace Autodor.Modules.Contractors.Infrastructure.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddModule<ContractorsDbContext>(
            configureDbContext: (serviceProvider, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ContractorsConnection"));
            }
        );

        // Rejestracja interfejsów DbContext
        services.AddScoped<IContractorsWriteDbContext>(provider => provider.GetRequiredService<ContractorsDbContext>());
        services.AddScoped<IContractorsReadDbContext>(provider => provider.GetRequiredService<ContractorsDbContext>());

        return services;
    }
}