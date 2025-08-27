using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

public record ExcludeOrderCommand(string OrderNumber, string Reason) : IRequest<bool>;