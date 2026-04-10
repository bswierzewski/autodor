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
using BuildingBlocks.Infrastructure.Soap.Builders;
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

        // Products integration uses cache because the payload changes relatively infrequently.
        services.AddScoped<IProductsClient, ProductsClient>();
        services.AddSoap(() => new ProductsSoapClient(ProductsSoapClient.EndpointConfiguration.ProductsSoap),
            soap => soap
                .AddCache(TimeSpan.FromDays(2))
                .AddResilience()
                .AddLogging());

        // Orders import should be resilient and observable, but should not reuse stale responses.
        services.AddScoped<IDistributorsSalesClient, DistributorsSalesClient>();
        services.AddSoap(() => new DistributorsSalesServiceClient(DistributorsSalesServiceClient.EndpointConfiguration.BasicHttpBinding_IDistributorsSalesService_soap),
            soap => soap
                .AddResilience()
                .AddLogging());

        services.AddHostedService<ProductsSyncWorker>();

        return services;
    }
}
