using System.Reflection;
using Autodor.Modules.Contractors.Application.Module;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Infrastructure;

namespace Autodor.Modules.Contractors.Infrastructure.Module;

/// <summary>
/// Provides dependency injection configuration for the Contractors Infrastructure layer.
/// This static class centralizes the registration of all infrastructure services including
/// database context, Entity Framework configuration, and CQRS interface implementations.
/// Follows the modular architecture pattern by encapsulating infrastructure-specific dependencies.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure layer services for the Contractors module into the dependency injection container.
    /// This method configures the database context, Entity Framework services, CQRS interfaces, and other
    /// infrastructure concerns necessary for the contractors domain to function properly.
    /// </summary>
    /// <param name="services">The service collection to register infrastructure dependencies into</param>
    /// <param name="configuration">The application configuration containing connection strings and settings</param>
    /// <returns>The service collection with registered infrastructure services for method chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MediatR for any infrastructure-level handlers or behaviors
        // This enables infrastructure event handling and cross-cutting concerns
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Configure and register the Entity Framework database context
        // Uses PostgreSQL as the database provider with connection string from configuration
        services.AddModule<ContractorsDbContext>(
            configureDbContext: (serviceProvider, options) =>
            {
                // Configure PostgreSQL connection for contractor data persistence
                // The connection string should be specified in application configuration
                options.UseNpgsql(configuration.GetConnectionString("ContractorsConnection"));
            }
        );

        // Register CQRS database context interfaces
        // These provide separated concerns for read and write operations while using the same context
        // Write context: Enables change tracking, transactions, and entity modifications
        services.AddScoped<IContractorsWriteDbContext>(provider => provider.GetRequiredService<ContractorsDbContext>());
        
        // Read context: Provides no-tracking queries for optimal read performance
        services.AddScoped<IContractorsReadDbContext>(provider => provider.GetRequiredService<ContractorsDbContext>());

        // Return services for fluent configuration chaining
        // Allows additional infrastructure configuration to be chained after this call
        return services;
    }
}