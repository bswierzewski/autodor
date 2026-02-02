using Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Options;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
