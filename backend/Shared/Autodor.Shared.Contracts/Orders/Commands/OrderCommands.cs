using MediatR;

namespace Autodor.Shared.Contracts.Orders.Commands;

public record ExcludeOrderCommand(string OrderNumber, string Reason) : IRequest<bool>;