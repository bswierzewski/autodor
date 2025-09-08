using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDate;

public record GetOrdersByDateQuery(DateTime Date) : IRequest<IEnumerable<Order>>;