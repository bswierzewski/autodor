using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;

public class CreateBulkInvoicesCommandHandler : IRequestHandler<CreateBulkInvoicesCommand, IEnumerable<Guid>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateBulkInvoicesCommandHandler> _logger;

    public CreateBulkInvoicesCommandHandler(IMediator mediator, ILogger<CreateBulkInvoicesCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IEnumerable<Guid>> Handle(CreateBulkInvoicesCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}