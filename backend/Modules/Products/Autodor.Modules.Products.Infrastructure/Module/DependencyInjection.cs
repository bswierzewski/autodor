using System.Reflection;
using Autodor.Modules.Products.Application.Module;
using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Infrastructure.Module;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Abstractions;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Services;
using Autodor.Modules.Products.Infrastructure.Persistence;
using BuildingBlocks.Application;
using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure.Module;

/// <summary>
/// Configures dependency injection for the Products infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services including Entity Framework DbContext, external service clients, and module configurator.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">Configuration containing connection strings and external service settings.</param>
    /// <param name="configure">Optional configurator for customizing module setup.</param>
    /// <returns>The configured service collection for method chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ProductsModuleConfigurator>? configure = null)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services
            .AddMigrationService<ProductsDbContext>()
            .AddAuditableEntityInterceptor()
            .AddDomainEventDispatchInterceptor();
            
        services.AddDbContext<ProductsDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ProductsConnection"))
                   .AddInterceptors(sp.GetServices<Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor>());
        });

        services.AddScoped<IProductsWriteDbContext>(provider => provider.GetRequiredService<ProductsDbContext>());
        services.AddScoped<IProductsReadDbContext>(provider => provider.GetRequiredService<ProductsDbContext>());

        services.Configure<PolcarProductsOptions>(configuration.GetSection(PolcarProductsOptions.SectionName));
        services.AddScoped(provider => new ExternalServices.Polcar.Generated.ProductsSoapClient(ExternalServices.Polcar.Generated.ProductsSoapClient.EndpointConfiguration.ProductsSoap));
        services.AddScoped<IPolcarProductService, PolcarProductService>();

        if (configure is not null)
        {
            var configurator = new ProductsModuleConfigurator(services);
            configure(configurator);
        }

        return services;
    }
}