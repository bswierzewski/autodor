using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Infrastructure.Interceptors;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Orders.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration)
    {
        // Rejestracja interceptorów - EF Core automatycznie je wykryje
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<DispatchDomainEventsInterceptor>();
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<OrdersDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("OrdersConnection")));

        // Rejestracja Repository + UnitOfWork
        services.AddScoped<IExcludedOrderRepository, ExcludedOrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPolcarDistributorsSalesService, Services.PolcarDistributorsSalesService>();

        return services;
    }
}