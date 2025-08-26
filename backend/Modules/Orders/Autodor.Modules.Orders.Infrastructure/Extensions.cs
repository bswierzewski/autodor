using System.Reflection;
using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure;

namespace Autodor.Modules.Orders.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration)
    {        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddDbContext<OrdersDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("OrdersConnection"));

            // This adds AuditableEntityInterceptor and DispatchDomainEventsInterceptor
            options.AddInterceptors(serviceProvider);
        });

        // 3. Register repositories and UnitOfWork
        services.AddRepositories<OrdersDbContext>();

        services.AddScoped<IExcludedOrderRepository, ExcludedOrderRepository>();
        services.AddScoped<IPolcarDistributorsSalesService, Services.PolcarDistributorsSalesService>();

        return services;
    }
}