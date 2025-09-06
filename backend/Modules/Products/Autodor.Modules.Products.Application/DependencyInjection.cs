using System.Reflection;
using Autodor.Modules.Products.Application.API;
using Autodor.Shared.Contracts.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // Register Products API for external communication
        services.AddScoped<IProductsAPI, ProductsAPI>();
        
        return services;
    }
}