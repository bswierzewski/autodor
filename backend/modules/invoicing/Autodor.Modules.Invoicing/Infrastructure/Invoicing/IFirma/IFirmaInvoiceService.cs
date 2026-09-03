using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Extensions;
using BuildingBlocks.Core.Exceptions;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma;

public class IFirmaInvoiceService(IIFirmaHttpClient httpClient) : IInvoiceService
{
    public async Task CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var iFirmaInvoice = invoice.ToIFirmaInvoice();
        var result = await httpClient.CreateInvoiceAsync(iFirmaInvoice, cancellationToken);

        if (!result.Response.IsSuccess)
            throw new DomainException(result.Response.Message ?? "Nieznany błąd z API iFirma.");
    }
}
