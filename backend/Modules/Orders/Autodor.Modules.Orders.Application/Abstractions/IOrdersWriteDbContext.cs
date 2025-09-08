using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Application.Abstractions;

/// <summary>
/// Defines the contract for write database operations within the Orders module.
/// This interface provides modification capabilities for order-related data persistence.
/// Implements the CQRS pattern by separating write operations from read operations.
/// </summary>
public interface IOrdersWriteDbContext
{
    /// <summary>
    /// Gets the DbSet for managing excluded order entities.
    /// Provides add, update, and delete operations for order exclusions.
    /// </summary>
    DbSet<ExcludedOrder> ExcludedOrders { get; }

    /// <summary>
    /// Asynchronously saves all pending changes to the database.
    /// This method commits the current transaction and ensures data persistence.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}