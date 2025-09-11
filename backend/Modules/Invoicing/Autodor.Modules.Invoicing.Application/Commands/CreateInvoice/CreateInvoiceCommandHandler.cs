using Autodor.Shared.Contracts.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;
    private readonly IProductsAPI _productsApi;

    public CreateInvoiceCommandHandler(
        IMediator mediator,
        ILogger<CreateInvoiceCommandHandler> logger,
        IProductsAPI productsApi)
    {
        _mediator = mediator;
        _logger = logger;
        _productsApi = productsApi;
    }

    public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}