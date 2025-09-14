using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDateRange;

/// <summary>
/// Query to retrieve all orders within a specified date range from external systems.
/// </summary>
/// <param name="DateFrom">The start date of the range (inclusive).</param>
/// <param name="DateTo">The end date of the range (inclusive).</param>
public record GetOrdersByDateRangeQuery(DateTime DateFrom, DateTime DateTo) : IRequest<IEnumerable<Order>>;