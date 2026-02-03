using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Options;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public class ProductsService(IOptions<ProductsOptions> options) : IProductsService
{
    private readonly ProductsOptions _options = options.Value;
}
