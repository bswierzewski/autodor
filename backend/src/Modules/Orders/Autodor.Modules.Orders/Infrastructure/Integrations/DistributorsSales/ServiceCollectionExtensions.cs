using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.ServiceReference;
using BuildingBlocks.Infrastructure.Soap;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;

/// <summary>
/// Extension methods for registering DistributorsSales integration services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds DistributorsSales integration services including SOAP invoker with resilience and logging, and service implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDistributorsSales(this IServiceCollection services)
    {
        // Register SOAP invoker with resilience and logging using builder pattern
        services.AddSoapInvoker<DistributorsSalesServiceClient>(() =>
                new DistributorsSalesServiceClient(DistributorsSalesServiceClient.EndpointConfiguration.BasicHttpBinding_IDistributorsSalesService_soap))
            .AddResilience()
            .AddLogging()
            .Build();

        // Register service implementation
        services.AddSingleton<IDistributorsSalesService, DistributorsSalesService>();

        return services;
    }
}
