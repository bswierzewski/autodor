using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

public record ExcludeOrderCommand(string Number) : IRequest<bool>;