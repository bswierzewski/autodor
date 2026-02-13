using Autodor.Modules.Invoicing.Domain.Aggregates;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Extensions;
using ErrorOr;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma;

public class IFirmaInvoiceService(IFirmaHttpClient httpClient) : IInvoiceService
{
    public async Task<ErrorOr<Success>> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var iFirmaInvoice = invoice.ToIFirmaInvoice();
        var result = await httpClient.CreateInvoiceAsync(iFirmaInvoice, cancellationToken);
        return result.IsError ? result.Errors : Result.Success;
    }
}
