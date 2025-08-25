using Autodor.Modules.Products.Domain.Abstractions;
using Autodor.Modules.Products.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddProducts(this IServiceCollection services, IConfiguration configuration)
    {
        // Rejestracja opcji konfiguracyjnych
        services.Configure<PolcarOptions>(configuration.GetSection("Polcar"));

        // Rejestracja cache'a
        services.AddMemoryCache();

        // Rejestracja serwisów
        services.AddScoped<IPolcarProductsService, Services.PolcarProductsService>();

        return services;
    }
}