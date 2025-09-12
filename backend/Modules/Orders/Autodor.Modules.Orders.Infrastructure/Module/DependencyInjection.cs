using System.Reflection;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application;
using BuildingBlocks.Infrastructure;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Options;

namespace Autodor.Modules.Orders.Infrastructure.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services
            .AddMigrationService<OrdersDbContext>()
            .AddAuditableEntityInterceptor()
            .AddDomainEventDispatchInterceptor();
            
        services.AddDbContext<OrdersDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("OrdersConnection"))
                   .AddInterceptors(sp.GetServices<Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor>());
        });

        services.AddScoped<IOrdersWriteDbContext>(provider => provider.GetRequiredService<OrdersDbContext>());
        services.AddScoped<IOrdersReadDbContext>(provider => provider.GetRequiredService<OrdersDbContext>());

        services.Configure<PolcarSalesOptions>(configuration.GetSection(PolcarSalesOptions.SectionName));
        services.AddScoped(provider => new DistributorsSalesServiceSoapClient(DistributorsSalesServiceSoapClient.EndpointConfiguration.DistributorsSalesServiceSoap));
        services.AddScoped<IOrdersRepository, PolcarOrderRepository>();

        return services;
    }
}