using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Services;

public class IFirmaInvoiceService(IFirmaHttpClient httpClient) : IInvoiceService
{
    public async Task<bool> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var iFirmaInvoice = invoice.ToIFirmaInvoice();

        var response = await httpClient.CreateInvoiceAsync(iFirmaInvoice, cancellationToken);

        if (!response.Response.IsSuccess)
            throw new InvalidOperationException($"Failed to create invoice in iFirma. Status code: {response.Response.StatusCode}, Message: {response.Response.Message}");

        return true;
    }
}
