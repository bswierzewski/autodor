using Autodor.Modules.Products.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Application.Abstractions;

/// <summary>
/// Provides write access to the Products database context for modifying product data.
/// </summary>
public interface IProductsWriteDbContext
{
    /// <summary>
    /// Gets the DbSet for products to enable add, update, and delete operations.
    /// </summary>
    DbSet<Product> Products { get; }

    /// <summary>
    /// Saves all pending changes to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The number of entities written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}