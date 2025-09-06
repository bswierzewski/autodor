using Autodor.Modules.Products.Domain.Aggregates;

namespace Autodor.Modules.Products.Application.Abstractions;

public interface IProductsReadDbContext
{
    IQueryable<Product> Products { get; }
}