using Autodor.Modules.Products.Domain.ValueObjects;

namespace Autodor.Modules.Products.Domain.Abstractions;

public interface IProductRepository
{
    Task<Product> GetProductAsync(string partNumber);
    Task<IEnumerable<Product>> GetProductsAsync(IEnumerable<string> partNumbers);
}