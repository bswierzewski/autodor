using Autodor.Modules.Orders.Domain.Aggregates;

namespace Autodor.Modules.Orders.Application.Abstractions;

/// <summary>
/// Provides read-only access to the orders database context.
/// </summary>
public interface IOrdersReadDbContext
{
    /// <summary>
    /// Gets a queryable collection of excluded orders for read operations.
    /// </summary>
    IQueryable<ExcludedOrder> ExcludedOrders { get; }
}