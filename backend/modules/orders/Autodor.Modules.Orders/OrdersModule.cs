using Autodor.Modules.Orders.Features.ExcludeOrder;
using Autodor.Modules.Orders.Features.ExcludeOrderItem;
using Autodor.Modules.Orders.Features.GenerateDeliveryNote;
using Autodor.Modules.Orders.Features.GetOrder;
using Autodor.Modules.Orders.Features.GetOrders;
using Autodor.Modules.Orders.Infrastructure.BackgroundJobs;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.ServiceReference;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.ServiceReference;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Modules.Extensions;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using BuildingBlocks.Soap.Builders;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Autodor.Modules.Orders;

public sealed class OrdersModule : IModuleEndpoint, IModuleMigration
{
    public string Name => "Orders";

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ExcludeOrderEndpoint.Map(endpoints);
        ExcludeOrderItemEndpoint.Map(endpoints);
        GenerateDeliveryNoteEndpoint.Map(endpoints);
        GetOrderEndpoint.Map(endpoints);
        GetOrdersEndpoint.Map(endpoints);
    }

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatedOptions<ProductsOptions>(configuration, ProductsOptions.SectionName);
        services.AddValidatedOptions<DistributorsSalesOptions>(configuration, DistributorsSalesOptions.SectionName);
        services.AddPostgres<OrdersDbContext>(OrdersDbContext.Schema);

        // Configure QuestPDF license (Community license for organizations with less than $1M USD annual gross revenue)
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddMemoryCache();

        // Register services
        services.AddScoped<IOrderService, OrderService>();

        // Products integration uses cache because the payload changes relatively infrequently.
        services.AddScoped<IProductsClient, ProductsClient>();
        services.AddSoap(() => new ProductsSoapClient(ProductsSoapClient.EndpointConfiguration.ProductsSoap),
            soap => soap
                .AddResilience()
                .AddLogging());

        // Orders can change during the day, so keep their cached responses short-lived.
        services.AddScoped<IDistributorsSalesClient, DistributorsSalesClient>();
        services.AddSoap(() => new DistributorsSalesServiceClient(DistributorsSalesServiceClient.EndpointConfiguration.BasicHttpBinding_IDistributorsSalesService_soap),
            soap => soap
                .AddCache(TimeSpan.FromMinutes(15))
                .AddResilience()
                .AddLogging());

        services.AddHostedService<ProductsSyncWorker>();
    }

    public Task MigrateAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        => services.MigrateDatabaseAsync<OrdersDbContext>(cancellationToken);
}
