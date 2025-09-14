using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Infrastructure.Services;

public class InFaktInvoiceService : IInvoiceService
{
    private readonly ILogger<InFaktInvoiceService> _logger;

    public InFaktInvoiceService(ILogger<InFaktInvoiceService> logger)
    {
        _logger = logger;
    }

    public Task<Guid> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _logger.LogError("inFakt integration not implemented yet");
        throw new NotImplementedException("inFakt integration not implemented yet");
    }
}