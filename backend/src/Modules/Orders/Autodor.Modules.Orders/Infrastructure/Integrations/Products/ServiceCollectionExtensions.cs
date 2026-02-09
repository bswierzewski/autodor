using Autodor.Modules.Orders.Infrastructure.Integrations.Products.ServiceReference;
using BuildingBlocks.Infrastructure.Soap;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.Products;

/// <summary>
/// Extension methods for registering Products integration services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Products integration services including SOAP invoker with resilience and logging, and service implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddProducts(this IServiceCollection services)
    {
        // Register SOAP invoker with resilience and logging using builder pattern
        services.AddSoapInvoker<ProductsSoapClient>(() =>
                new ProductsSoapClient(ProductsSoapClient.EndpointConfiguration.ProductsSoap))
            .AddResilience()
            .AddLogging()
            .Build();

        // Register service implementation
        services.AddSingleton<IProductsService, ProductsService>();

        return services;
    }
}
