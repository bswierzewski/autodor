using Autodor.Modules.Contractors.Domain.Aggregates;

namespace Autodor.Modules.Contractors.Application.Abstractions;

/// <summary>
/// Defines the contract for read-only database operations in the Contractors module.
/// This interface follows the CQRS (Command Query Responsibility Segregation) pattern by providing
/// a dedicated context for query operations, enabling optimized read scenarios and better separation of concerns.
/// Used by query handlers to retrieve contractor data without the overhead of change tracking.
/// </summary>
public interface IContractorsReadDbContext
{
    /// <summary>
    /// Gets the queryable collection of contractors for read-only operations.
    /// This property enables efficient querying with LINQ expressions while maintaining
    /// separation between read and write operations. The IQueryable interface allows
    /// for database-level query optimization and projection capabilities.
    /// </summary>
    IQueryable<Contractor> Contractors { get; }
}