using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDate;

/// <summary>
/// Query to retrieve all orders for a specific date from external systems.
/// </summary>
/// <param name="Date">The date for which to retrieve orders.</param>
public record GetOrdersByDateQuery(DateTime Date) : IRequest<IEnumerable<Order>>;