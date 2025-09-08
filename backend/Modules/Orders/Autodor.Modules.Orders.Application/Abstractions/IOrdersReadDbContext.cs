using Autodor.Modules.Orders.Domain.Aggregates;

namespace Autodor.Modules.Orders.Application.Abstractions;

/// <summary>
/// Defines the contract for read-only database operations within the Orders module.
/// This interface provides query access to order-related data without modification capabilities.
/// Implements the CQRS pattern by separating read operations from write operations.
/// </summary>
public interface IOrdersReadDbContext
{
    /// <summary>
    /// Gets a queryable collection of excluded orders for read operations.
    /// Used for filtering and validation to prevent processing of excluded orders.
    /// </summary>
    IQueryable<ExcludedOrder> ExcludedOrders { get; }
}