using Autodor.Modules.Orders.Infrastructure.Integrations.Products.ServiceReference;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.Products.Factories;

/// <summary>
/// Factory for creating ProductsSoapClient instances.
/// </summary>
public class ProductsSoapClientFactory : IProductsSoapClientFactory
{
    /// <inheritdoc />
    public ProductsSoapClient Create() 
        => new(ProductsSoapClient.EndpointConfiguration.ProductsSoap);
}
