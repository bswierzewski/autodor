using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;
using ErrorOr;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Services;

public class IFirmaInvoiceService(IFirmaHttpClient httpClient) : IInvoiceService
{
    public async Task<ErrorOr<bool>> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var iFirmaInvoice = invoice.ToIFirmaInvoice();

        var result = await httpClient.CreateInvoiceAsync(iFirmaInvoice, cancellationToken);

        if (result.IsError)
            return result.Errors;

        return true;
    }
}
