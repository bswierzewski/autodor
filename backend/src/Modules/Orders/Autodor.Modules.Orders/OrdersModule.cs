using Autodor.Modules.Orders.Infrastructure.BackgroundJobs;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.ServiceReference;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.ServiceReference;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Soap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Autodor.Modules.Orders;

public static class OrdersModule
{
    public static readonly string Name = "Orders";

    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModule(configuration, Name)
            .AddOptions(cfg =>
            {
                cfg.ConfigureOptions<ProductsOptions>();
                cfg.ConfigureOptions<DistributorsSalesOptions>();
            })
            .AddPostgres<OrdersDbContext>()
            .Build();

        // Configure QuestPDF license (Community license for organizations with less than $1M USD annual gross revenue)
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddMemoryCache();

        // Register services
        services.AddScoped<IOrderService, OrderService>();

        // Register service implementation
        services.AddScoped<IProductsClient, ProductsClient>();
        // Register SOAP invoker with caching, resilience and logging using builder pattern
        services.AddSoapInvoker(() =>
                new ProductsSoapClient(ProductsSoapClient.EndpointConfiguration.ProductsSoap))
            .AddCache(TimeSpan.FromDays(2))
            .AddResilience()
            .AddLogging()
            .Build();

        // Register service implementation
        services.AddScoped<IDistributorsSalesClient, DistributorsSalesClient>();
        // Register SOAP invoker with resilience and logging using builder pattern
        services.AddSoapInvoker(() =>
                new DistributorsSalesServiceClient(DistributorsSalesServiceClient.EndpointConfiguration.BasicHttpBinding_IDistributorsSalesService_soap))
            .AddResilience()
            .AddLogging()
            .Build();

        services.AddHostedService<ProductsSyncWorker>();

        return services;
    }
}
