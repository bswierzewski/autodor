using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Application.Abstractions;

/// <summary>
/// Provides write access to the orders database context for data modifications.
/// </summary>
public interface IOrdersWriteDbContext
{
    /// <summary>
    /// Gets the DbSet for excluded orders to perform write operations.
    /// </summary>
    DbSet<ExcludedOrder> ExcludedOrders { get; }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}