using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Shared.Contracts.Invoicing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Orders.Application.EventHandlers;

/// <summary>
/// Handles invoice creation events by automatically excluding the associated orders from future processing.
/// </summary>
public class InvoiceCreatedEventHandler : INotificationHandler<InvoiceCreatedEvent>
{
    private readonly IOrdersWriteDbContext _writeDbContext;
    private readonly ILogger<InvoiceCreatedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the InvoiceCreatedEventHandler.
    /// </summary>
    /// <param name="writeDbContext">The database context for write operations.</param>
    /// <param name="logger">The logger for recording event handling activities.</param>
    public InvoiceCreatedEventHandler(
        IOrdersWriteDbContext writeDbContext,
        ILogger<InvoiceCreatedEventHandler> logger)
    {
        _writeDbContext = writeDbContext;
        _logger = logger;
    }

    /// <summary>
    /// Processes the invoice creation event by excluding all associated orders.
    /// </summary>
    /// <param name="notification">The invoice created event containing order IDs to exclude.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    public async Task Handle(InvoiceCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing InvoiceCreatedEvent for invoice {InvoiceNumber} with {OrderCount} orders",
            notification.InvoiceNumber, notification.OrderIds.Count());

        // Exclude each order associated with the created invoice
        foreach (var orderId in notification.OrderIds)
        {
            var excludedOrder = new ExcludedOrder(
                orderId,
                notification.CreatedDate
            );

            await _writeDbContext.ExcludedOrders.AddAsync(excludedOrder, cancellationToken);
        }

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully excluded {OrderCount} orders after invoice creation",
            notification.OrderIds.Count());
    }
}