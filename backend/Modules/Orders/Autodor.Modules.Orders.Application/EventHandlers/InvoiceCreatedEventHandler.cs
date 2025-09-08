using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Shared.Contracts.Invoicing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Orders.Application.EventHandlers;

/// <summary>
/// Handles invoice creation events by excluding related orders from further processing.
/// This handler implements the business rule that once an invoice is created for orders,
/// those orders should be excluded from duplicate processing to maintain data integrity.
/// Part of the cross-module communication between Invoicing and Orders modules.
/// </summary>
public class InvoiceCreatedEventHandler : INotificationHandler<InvoiceCreatedEvent>
{
    private readonly IOrdersWriteDbContext _writeDbContext;
    private readonly ILogger<InvoiceCreatedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the InvoiceCreatedEventHandler.
    /// Sets up dependencies for database operations and logging.
    /// </summary>
    /// <param name="writeDbContext">Database context for persisting order exclusions</param>
    /// <param name="logger">Logger for tracking event processing and business operations</param>
    public InvoiceCreatedEventHandler(
        IOrdersWriteDbContext writeDbContext,
        ILogger<InvoiceCreatedEventHandler> logger)
    {
        _writeDbContext = writeDbContext;
        _logger = logger;
    }

    /// <summary>
    /// Processes the invoice creation event by excluding all associated orders.
    /// This ensures business rule compliance that invoiced orders cannot be processed again.
    /// </summary>
    /// <param name="notification">The event containing invoice details and related order IDs</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    public async Task Handle(InvoiceCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Log the start of processing for business monitoring and debugging
        _logger.LogInformation("Processing InvoiceCreatedEvent for invoice {InvoiceNumber} with {OrderCount} orders",
            notification.InvoiceNumber, notification.OrderIds.Count());

        // Process each order ID to create exclusion records
        // This prevents duplicate invoice generation for the same orders
        foreach (var orderId in notification.OrderIds)
        {
            // Create exclusion record using the invoice creation date for audit consistency
            // This maintains temporal integrity between invoice creation and order exclusion
            var excludedOrder = new ExcludedOrder(
                orderId,
                notification.CreatedDate
            );

            // Add to database context for batch processing efficiency
            await _writeDbContext.ExcludedOrders.AddAsync(excludedOrder, cancellationToken);
        }

        // Commit all exclusions in a single transaction for data consistency
        // This ensures either all orders are excluded or none, maintaining business integrity
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        // Log successful completion for business monitoring and audit purposes
        _logger.LogInformation("Successfully excluded {OrderCount} orders after invoice creation",
            notification.OrderIds.Count());
    }
}