using Autodor.Modules.Contractors.Domain.Aggregates;

namespace Autodor.Modules.Contractors.Application.Abstractions;

/// <summary>
/// Read-only database context abstraction for contractor queries with no-tracking behavior.
/// </summary>
public interface IContractorsReadDbContext
{
    /// <summary>
    /// Provides queryable access to contractors for read operations.
    /// </summary>
    IQueryable<Contractor> Contractors { get; }
}