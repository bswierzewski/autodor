using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDateRange;

public record GetOrdersByDateRangeQuery(DateTime DateFrom, DateTime DateTo) : IRequest<IEnumerable<Order>>;