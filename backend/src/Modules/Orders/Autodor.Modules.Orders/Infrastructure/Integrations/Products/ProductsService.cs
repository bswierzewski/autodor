using Autodor.Modules.Orders.Abstractions.Integrations.Products;
using Autodor.Modules.Orders.Abstractions.Integrations.Products.Models;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Options;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public class ProductsService(IOptions<ProductsOptions> options) : IProductsService
{
    private readonly ProductsOptions _options = options.Value;

    public Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        throw new NotImplementedException();
    }
}
