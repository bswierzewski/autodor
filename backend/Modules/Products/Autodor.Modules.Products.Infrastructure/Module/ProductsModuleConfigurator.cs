using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure.Module;

/// <summary>
/// Fluent configurator for Products module optional features and services.
/// Allows selective enabling of additional functionality like background synchronization.
/// </summary>
public class ProductsModuleConfigurator
{
    /// <summary>
    /// Gets the service collection for registering additional services.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the ProductsModuleConfigurator.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    internal ProductsModuleConfigurator(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Enables automatic product synchronization from external Polcar service.
    /// Registers a background hosted service that periodically updates the product catalog.
    /// </summary>
    /// <returns>The configurator instance for method chaining</returns>
    public ProductsModuleConfigurator AddSynchronization()
    {
        // Register background service for automated data synchronization
        // Business rationale: Keeps local product catalog current with supplier changes
        Services.AddHostedService<ProductsSynchronizationService>();
        return this;
    }
}