using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Shared.Contracts.Invoicing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Orders.Application.EventHandlers;

public class InvoiceCreatedEventHandler(
    IOrdersDbContext context,
    ILogger<InvoiceCreatedEventHandler> logger) : INotificationHandler<InvoiceCreatedEvent>
{
    public async Task Handle(InvoiceCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing InvoiceCreatedEvent for invoice {InvoiceNumber} with {OrderCount} orders",
            notification.InvoiceNumber, notification.OrderIds.Count());

        // Exclude each order associated with the created invoice
        foreach (var orderId in notification.OrderIds)
        {
            var excludedOrder = new ExcludedOrder(
                orderId,
                notification.CreatedDate
            );

            await context.ExcludedOrders.AddAsync(excludedOrder, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully excluded {OrderCount} orders after invoice creation",
            notification.OrderIds.Count());
    }
}