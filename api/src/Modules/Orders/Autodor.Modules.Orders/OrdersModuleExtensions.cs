using Autodor.Modules.Orders.Domain;
using Autodor.Modules.Orders.Infrastructure.Database;
using Autodor.Shared.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Orders;

public static class OrdersModuleExtensions
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModule(configuration, Module.Name)
            .AddOptions(opts => { })
            .AddPostgres<OrdersDbContext>();

        return services;
    }
}
