using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Interfaces;
using Autodor.Shared.Contracts.Invoicing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Orders.Application.EventHandlers;

public class InvoiceCreatedEventHandler : INotificationHandler<InvoiceCreatedEvent>
{
    private readonly IOrdersWriteDbContext _writeDbContext;
    private readonly ILogger<InvoiceCreatedEventHandler> _logger;

    public InvoiceCreatedEventHandler(
        IOrdersWriteDbContext writeDbContext,
        ILogger<InvoiceCreatedEventHandler> logger)
    {
        _writeDbContext = writeDbContext;
        _logger = logger;
    }

    public async Task Handle(InvoiceCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing InvoiceCreatedEvent for invoice {InvoiceNumber} with {OrderCount} orders",
            notification.InvoiceNumber, notification.OrderNumbers.Count());

        foreach (var orderNumber in notification.OrderNumbers)
        {
            var excludedOrder = new ExcludedOrder(
                orderNumber,
                $"Automatically excluded - invoice {notification.InvoiceNumber} created",
                notification.CreatedDate
            );

            await _writeDbContext.ExcludedOrders.AddAsync(excludedOrder, cancellationToken);
        }

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully excluded {OrderCount} orders after invoice creation",
            notification.OrderNumbers.Count());
    }
}