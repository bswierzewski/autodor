using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma;

public class InvoiceService : IInvoiceService
{
    private readonly ILogger<InvoiceService> _logger;
    private readonly IFirmaClient _client;

    public InvoiceService(
        ILogger<InvoiceService> logger,
        IFirmaClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<string> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        try
        {
            var invoiceDto = invoice.ToInvoiceDto();
            var iFirmaResponse = await _client.CreateInvoiceAsync(invoiceDto, cancellationToken);

            if (iFirmaResponse.Response?.Code == 0 && !string.IsNullOrEmpty(iFirmaResponse.Response.Identifier))
            {
                var invoiceId = iFirmaResponse.Response.Identifier;
                _logger.LogInformation("Invoice created successfully for contractor: {ContractorName} with ID: {InvoiceId}", invoice.Contractor.Name, invoiceId);
                return invoiceId;
            }
            else
            {
                var errorMsg = iFirmaResponse.Response?.Information ?? "Unknown error";
                throw new InvalidOperationException($"Failed to create invoice: {errorMsg}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice for contractor: {ContractorName}", invoice.Contractor.Name);
            throw;
        }
    }
}