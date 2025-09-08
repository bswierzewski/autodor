using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using MediatR;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

/// <summary>
/// Handles the execution of order exclusion commands.
/// This handler implements the business logic for permanently excluding orders from processing,
/// creating an audit trail and ensuring excluded orders cannot be processed accidentally.
/// </summary>
public class ExcludeOrderCommandHandler(IOrdersWriteDbContext writeDbContext, TimeProvider timeProvider)
    : IRequestHandler<ExcludeOrderCommand, bool>
{
    private readonly IOrdersWriteDbContext _writeDbContext = writeDbContext;

    /// <summary>
    /// Processes the order exclusion command by creating an exclusion record.
    /// This operation is permanent and creates an audit trail for business compliance.
    /// </summary>
    /// <param name="request">The command containing the order identifier to exclude</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>True if the order was successfully excluded, false otherwise</returns>
    public async Task<bool> Handle(ExcludeOrderCommand request, CancellationToken cancellationToken)
    {
        // Create the exclusion aggregate with current timestamp for audit purposes
        // Using TimeProvider ensures testability and allows for time mocking in unit tests
        var excludedOrder = new ExcludedOrder(request.OrderId, timeProvider.GetUtcNow());

        // Persist the exclusion decision to maintain business rule enforcement
        // This creates a permanent record preventing future processing of this order
        await _writeDbContext.ExcludedOrders.AddAsync(excludedOrder, cancellationToken);
        
        // Commit the transaction to ensure the exclusion is permanently recorded
        // This guarantees that the business rule is enforced across all system operations
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        // Return success indicator for the calling application layer
        return true;
    }
}