using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using MediatR;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

/// <summary>
/// Handles the exclusion of orders by creating an excluded order record in the database.
/// </summary>
public class ExcludeOrderCommandHandler(IOrdersWriteDbContext writeDbContext, TimeProvider timeProvider)
    : IRequestHandler<ExcludeOrderCommand, Result<bool>>
{
    private readonly IOrdersWriteDbContext _writeDbContext = writeDbContext;

    /// <summary>
    /// Processes the exclude order command by creating a new excluded order entry.
    /// </summary>
    /// <param name="request">The exclude order command containing the order ID to exclude.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>True if the order was successfully excluded.</returns>
    public async Task<Result<bool>> Handle(ExcludeOrderCommand request, CancellationToken cancellationToken)
    {
        var excludedOrder = new ExcludedOrder(request.OrderId, timeProvider.GetUtcNow());

        await _writeDbContext.ExcludedOrders.AddAsync(excludedOrder, cancellationToken);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}