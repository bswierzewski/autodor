using Autodor.Modules.Products.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddProducts(this IServiceCollection services, IConfiguration configuration)
    {
        // Rejestracja cache'a
        services.AddMemoryCache();

        // Rejestracja serwisów
        services.AddScoped<IPolcarProductsService, Services.PolcarProductsService>();

        return services;
    }
}