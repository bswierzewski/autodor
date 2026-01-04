using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrderById;

public record GetOrderByIdQuery(string OrderId) : IRequest<Order?>;
