using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Shared.Contracts.Invoicing.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Domain.Abstractions;

namespace Autodor.Modules.Orders.Application.EventHandlers;

public class InvoiceCreatedEventHandler : INotificationHandler<InvoiceCreatedEvent>
{
    private readonly IRepository<ExcludedOrder> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InvoiceCreatedEventHandler> _logger;

    public InvoiceCreatedEventHandler(
        IRepository<ExcludedOrder> repository,
        IUnitOfWork unitOfWork,
        ILogger<InvoiceCreatedEventHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
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

            await _repository.AddAsync(excludedOrder);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully excluded {OrderCount} orders after invoice creation",
            notification.OrderNumbers.Count());
    }
}