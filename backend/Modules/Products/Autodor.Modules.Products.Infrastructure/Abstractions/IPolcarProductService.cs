using Autodor.Modules.Products.Domain.ValueObjects;

namespace Autodor.Modules.Products.Infrastructure.Abstractions;

public interface IPolcarProductService
{
    Task<IEnumerable<Product>> GetProductsAsync();
}