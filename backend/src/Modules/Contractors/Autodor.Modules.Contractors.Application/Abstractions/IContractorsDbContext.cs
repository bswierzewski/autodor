using Autodor.Modules.Contractors.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Abstractions;

/// <summary>
/// Database context abstraction for contractor operations.
/// </summary>
public interface IContractorsDbContext
{
    /// <summary>
    /// Provides access to the contractors collection.
    /// </summary>
    DbSet<Contractor> Contractors { get; }

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of entities written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
