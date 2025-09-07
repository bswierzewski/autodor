using Autodor.Modules.Products.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Application.Abstractions;

/// <summary>
/// Write-enabled database context interface for the Products module.
/// Supports CQRS pattern by providing full entity manipulation capabilities with change tracking.
/// </summary>
public interface IProductsWriteDbContext
{
    /// <summary>
    /// Gets the Products DbSet for full entity operations including add, update, and delete.
    /// Includes change tracking for write operations and transaction support.
    /// </summary>
    DbSet<Product> Products { get; }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the save operation</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}