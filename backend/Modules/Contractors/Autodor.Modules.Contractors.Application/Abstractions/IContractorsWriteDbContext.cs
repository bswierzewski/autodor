using Autodor.Modules.Contractors.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Abstractions;

/// <summary>
/// Defines the contract for write operations in the Contractors module database context.
/// This interface follows the CQRS pattern by providing a dedicated context for command operations,
/// enabling transactional integrity, change tracking, and optimized write scenarios.
/// Used by command handlers to persist contractor data modifications to the database.
/// </summary>
public interface IContractorsWriteDbContext
{
    /// <summary>
    /// Gets the DbSet for contractor entities, enabling create, update, and delete operations.
    /// This property provides full Entity Framework capabilities including change tracking,
    /// relationship management, and transaction support for contractor data modifications.
    /// </summary>
    DbSet<Contractor> Contractors { get; }

    /// <summary>
    /// Persists all pending changes to the database within a transaction.
    /// This method ensures atomicity of all contractor-related operations, maintaining data consistency
    /// and providing rollback capabilities in case of errors during the persistence process.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the save operation if needed</param>
    /// <returns>The number of entities affected by the save operation</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}