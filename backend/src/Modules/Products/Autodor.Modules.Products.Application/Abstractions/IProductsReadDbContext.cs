using Autodor.Modules.Products.Domain.Aggregates;

namespace Autodor.Modules.Products.Application.Abstractions;

/// <summary>
/// Provides read-only access to the Products database context for querying product data.
/// </summary>
public interface IProductsReadDbContext
{
    /// <summary>
    /// Gets the queryable collection of products for read operations.
    /// </summary>
    IQueryable<Product> Products { get; }
}