using System.Reflection;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Infrastructure;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Options;

namespace Autodor.Modules.Orders.Infrastructure.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddModule<OrdersDbContext>(
            configureDbContext: (serviceProvider, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("OrdersConnection"));
            }
        );

        // Rejestracja interfejsów DbContext
        services.AddScoped<IOrdersWriteDbContext>(provider => provider.GetRequiredService<OrdersDbContext>());
        services.AddScoped<IOrdersReadDbContext>(provider => provider.GetRequiredService<OrdersDbContext>());

        // Configure Polcar options
        services.Configure<PolcarSalesOptions>(configuration.GetSection(PolcarSalesOptions.SectionName));

        // Register SOAP client
        services.AddScoped(provider => new DistributorsSalesServiceSoapClient(DistributorsSalesServiceSoapClient.EndpointConfiguration.DistributorsSalesServiceSoap));

        // Register Polcar service
        services.AddScoped<IOrdersRepository, PolcarOrderRepository>();

        return services;
    }
}