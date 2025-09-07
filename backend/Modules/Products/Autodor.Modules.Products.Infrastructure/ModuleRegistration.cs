using Autodor.Modules.Products.Application.Module;
using Autodor.Modules.Products.Infrastructure.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure;

public static class ModuleRegistration
{
    public static IServiceCollection AddProducts(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ProductsModuleConfigurator>? configure = null)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration, configure);

        return services;
    }
}