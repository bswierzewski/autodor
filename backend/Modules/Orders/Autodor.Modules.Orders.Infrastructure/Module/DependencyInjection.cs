using System.Reflection;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application;
using BuildingBlocks.Infrastructure;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Options;

namespace Autodor.Modules.Orders.Infrastructure.Module;

/// <summary>
/// Provides dependency injection configuration for the Orders Infrastructure layer.
/// This class registers all infrastructure services including database contexts,
/// external service clients, and repository implementations required for the Orders module.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure layer services for the Orders module.
    /// Configures database contexts, external service integrations, and repository implementations.
    /// </summary>
    /// <param name="services">The service collection to add dependencies to</param>
    /// <param name="configuration">Application configuration for connection strings and settings</param>
    /// <returns>The service collection with registered Orders infrastructure services</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MediatR for infrastructure event handling and cross-cutting concerns
        // This enables infrastructure-level event processing and notification handling
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register Entity Framework interceptors for auditing and domain event dispatching
        services.AddAuditableEntityInterceptor();
        services.AddDomainEventDispatchInterceptor();

        // Configure and register the Entity Framework database context
        // Uses PostgreSQL as the database provider with connection string from configuration
        services.AddDbContext<OrdersDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("OrdersConnection"));
        });

        // Register CQRS-specific database context interfaces
        // This enables separation of read and write operations for better performance and maintainability
        services.AddScoped<IOrdersWriteDbContext>(provider => provider.GetRequiredService<OrdersDbContext>());
        services.AddScoped<IOrdersReadDbContext>(provider => provider.GetRequiredService<OrdersDbContext>());

        // Configure Polcar external service options from application settings
        // This binds configuration section to strongly-typed options for external service integration
        services.Configure<PolcarSalesOptions>(configuration.GetSection(PolcarSalesOptions.SectionName));

        // Register SOAP client for Polcar external service integration
        // This enables communication with the external Polcar system for order data retrieval
        services.AddScoped(provider => new DistributorsSalesServiceSoapClient(DistributorsSalesServiceSoapClient.EndpointConfiguration.DistributorsSalesServiceSoap));

        // Register repository implementation for order data access
        // This provides the concrete implementation for IOrdersRepository using Polcar as data source
        services.AddScoped<IOrdersRepository, PolcarOrderRepository>();

        return services;
    }
}