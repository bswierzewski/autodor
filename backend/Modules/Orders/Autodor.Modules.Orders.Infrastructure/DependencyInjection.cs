using System.Reflection;
using Autodor.Modules.Orders.Application;
using Autodor.Modules.Orders.Domain.Abstractions;
using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Orders.Infrastructure.Options;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure;

namespace Autodor.Modules.Orders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddDbContext<OrdersDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("OrdersConnection"));

            // This adds AuditableEntityInterceptor and DispatchDomainEventsInterceptor
            options.AddInterceptors(serviceProvider);
        });

        services.AddModuleContext<OrdersDbContext>(module =>
        {
            module.AddMigrations();
            module.AddRepository<ExcludedOrder>();
        });

        // Configure Polcar options
        services.Configure<PolcarSalesOptions>(configuration.GetSection(PolcarSalesOptions.SectionName));

        // Register SOAP client
        services.AddScoped<DistributorsSalesServiceSoapClient>(provider =>
            new DistributorsSalesServiceSoapClient(DistributorsSalesServiceSoapClient.EndpointConfiguration.DistributorsSalesServiceSoap));

        // Register Polcar service
        services.AddScoped<IOrderRepository, PolcarOrderRepository>();

        return services;
    }
}