using Autodor.Modules.Orders.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrderById;

public record GetOrderByIdQuery(string OrderId) : IRequest<ErrorOr<Order>>;
