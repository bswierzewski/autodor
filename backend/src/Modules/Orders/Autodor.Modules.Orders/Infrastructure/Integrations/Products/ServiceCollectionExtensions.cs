using Autodor.Modules.Orders.Infrastructure.Consts;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Factories;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.ServiceModel;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.Products;

/// <summary>
/// Extension methods for registering Products integration services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Products integration services including SOAP client factory, resilience pipeline, service, and cache.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddProducts(this IServiceCollection services)
    {
        // Register SOAP client factory
        services.AddSingleton<IProductsSoapClientFactory, ProductsSoapClientFactory>();

        // Register resilience pipeline for SOAP client with retry and timeout policies
        services.AddResiliencePipeline(KeyedServicesConsts.ProductsSoap, builder =>
        {
            builder
                .AddRetry(new Polly.Retry.RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder()
                        .Handle<CommunicationException>()
                        .Handle<TimeoutException>()
                        .Handle<EndpointNotFoundException>()
                })
                .AddTimeout(TimeSpan.FromSeconds(30));
        });

        // Register SOAP service
        services.AddSingleton<IProductsService, ProductsService>();

        return services;
    }
}
