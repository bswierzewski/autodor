using System.Reflection;
using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Services;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
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

        // Rejestracja serwisu do uruchamiania migracji
        services.AddHostedService<OrdersMigrationService>();
        
        // Configure Polcar options
        services.Configure<PolcarSalesOptions>(configuration.GetSection(PolcarSalesOptions.SectionName));
        
        // Register SOAP client
        services.AddScoped<DistributorsSalesServiceSoapClient>();
        
        // Register Polcar service
        services.AddScoped<IPolcarOrdersService, PolcarOrdersService>();

        return services;
    }
}