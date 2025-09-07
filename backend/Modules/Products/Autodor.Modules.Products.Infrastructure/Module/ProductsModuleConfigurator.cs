using Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure.Module;

public class ProductsModuleConfigurator
{
    public IServiceCollection Services { get; }

    internal ProductsModuleConfigurator(IServiceCollection services)
    {
        Services = services;
    }

    public ProductsModuleConfigurator AddSynchronization()
    {
        Services.AddHostedService<ProductsSynchronizationService>();
        return this;
    }
}