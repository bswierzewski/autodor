using Autodor.Modules.Products.Infrastructure.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Products.Infrastructure.Configuration;

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