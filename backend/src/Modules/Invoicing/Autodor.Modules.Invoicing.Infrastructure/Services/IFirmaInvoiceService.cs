using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Infrastructure.Services;

public class IFirmaInvoiceService : IInvoiceService
{
    private readonly ILogger<IFirmaInvoiceService> _logger;

    public IFirmaInvoiceService(ILogger<IFirmaInvoiceService> logger)
    {
        _logger = logger;
    }

    public Task<Guid> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _logger.LogError("iFirma integration not implemented yet");
        throw new NotImplementedException("iFirma integration not implemented yet");
    }
}