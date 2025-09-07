using Autodor.Modules.Products.Domain.Aggregates;

namespace Autodor.Modules.Products.Application.Abstractions;

/// <summary>
/// Read-only database context interface for the Products module.
/// Supports CQRS pattern by providing optimized query access without change tracking.
/// </summary>
public interface IProductsReadDbContext
{
    /// <summary>
    /// Gets the Products queryable collection optimized for read operations.
    /// Should be implemented with change tracking disabled for better performance.
    /// </summary>
    IQueryable<Product> Products { get; }
}