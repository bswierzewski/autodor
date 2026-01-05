using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrders;

public record GetOrdersQuery(DateTime From, DateTime? To = null) : IRequest<IEnumerable<Order>>;
