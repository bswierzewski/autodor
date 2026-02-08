using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales;
using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Options;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Options;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Services.Caching;
using Autodor.Modules.Orders.Infrastructure.Services.Orders;
using BuildingBlocks.Infrastructure.Extensions;
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

        // Register products cache
        services.AddSingleton<IProductsCache, ProductsCache>();

        // Register services
        services.AddScoped<IOrderService, OrderService>();

        // Register external integrations
        services.AddProducts();
        services.AddDistributorsSales();

        return services;
    }
}
