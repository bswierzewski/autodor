using MediatR;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

/// <summary>
/// Command to exclude an order from processing or invoicing.
/// </summary>
/// <param name="OrderId">The unique identifier of the order to exclude.</param>
public record ExcludeOrderCommand(string OrderId) : IRequest<Result<bool>>;