using Autodor.Modules.Products.Domain.Aggregates;

namespace Autodor.Modules.Products.Infrastructure.Abstractions;

public interface IPolcarProductService
{
    Task<IEnumerable<Product>> GetProductsAsync();
}