using System.Reflection;
using Autodor.Modules.Products.Domain.Abstractions;
using Autodor.Modules.Products.Infrastructure.Services;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;
using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Generated;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddProducts(this IServiceCollection services, IConfiguration configuration)
    {
        // Rejestracja MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Rejestracja opcji konfiguracyjnych
        services.Configure<PolcarProductsOptions>(configuration.GetSection(PolcarProductsOptions.SectionName));

        // Rejestracja cache'a
        services.AddMemoryCache();

        // Register SOAP client
        services.AddScoped<ProductsSoapClient>();
        
        // Rejestracja serwisów
        services.AddScoped<IPolcarProductsService, PolcarProductsService>();

        return services;
    }
}