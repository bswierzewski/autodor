using Autodor.Modules.Products.Domain.Aggregates;

namespace Autodor.Modules.Products.Application.Interfaces;

public interface IProductsReadDbContext
{
    IQueryable<Product> Products { get; }
}