using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure.Module;

/// <summary>
/// Provides fluent configuration for the Products module infrastructure services.
/// </summary>
public class ProductsModuleConfigurator
{
    /// <summary>
    /// Gets the service collection being configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the ProductsModuleConfigurator class.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    internal ProductsModuleConfigurator(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Enables automatic product synchronization from external systems via background service.
    /// </summary>
    /// <returns>The configurator instance for method chaining</returns>
    public ProductsModuleConfigurator AddSynchronization()
    {
        Services.AddHostedService<ProductsSynchronizationService>();
        return this;
    }
}