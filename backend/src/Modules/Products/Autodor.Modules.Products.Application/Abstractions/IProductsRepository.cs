using Autodor.Modules.Products.Domain.Entities;

namespace Autodor.Modules.Products.Application.Abstractions;

public interface IProductsRepository
{
    Product? GetProductByNumber(string number);
    IEnumerable<Product> GetProductsByNumbers(IEnumerable<string> numbers);
    void ReplaceAllProducts(IEnumerable<Product> products);
    int GetProductCount();
}