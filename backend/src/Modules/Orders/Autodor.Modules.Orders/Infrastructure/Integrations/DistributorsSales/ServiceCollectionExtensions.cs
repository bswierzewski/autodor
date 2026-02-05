using Autodor.Modules.Orders.Infrastructure.Consts;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Factories;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.ServiceModel;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;

/// <summary>
/// Extension methods for registering DistributorsSales integration services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds DistributorsSales integration services including SOAP client factory, resilience pipeline, and service implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDistributorsSales(this IServiceCollection services)
    {
        // Register SOAP client factory
        services.AddSingleton<IDistributorsSalesServiceClientFactory, DistributorsSalesServiceClientFactory>();

        // Register resilience pipeline for SOAP client with retry and timeout policies
        services.AddResiliencePipeline(KeyedServicesConsts.DistributorsSalesSoap, builder =>
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

        // Register service implementation
        services.AddScoped<IDistributorsSalesService, DistributorsSalesService>();

        return services;
    }
}
