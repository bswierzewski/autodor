using System.Reflection;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Generated;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Repositories;
using Autodor.Modules.Orders.Infrastructure.Services;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<IPdfDocumentService, PdfDocumentService>();

        // Rejestracja modułu dla systemu uprawnień
        services.AddSingleton<IModule, Module>();

        return services;
    }
}