using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

/// <summary>
/// Command to exclude an order from further processing.
/// This command triggers the business decision to mark an order as excluded,
/// preventing it from being processed through the normal order workflow.
/// Used when orders need to be filtered out based on business rules or manual decisions.
/// </summary>
/// <param name="OrderId">The unique identifier of the order to be excluded from processing</param>
public record ExcludeOrderCommand(string OrderId) : IRequest<bool>;