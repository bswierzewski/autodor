using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Extensions;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma;

public class IFirmaInvoiceService(IFirmaHttpClient httpClient) : IInvoiceService
{
    public async Task CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var iFirmaInvoice = invoice.ToIFirmaInvoice();

        await httpClient.CreateInvoiceAsync(iFirmaInvoice, cancellationToken);
    }
}
