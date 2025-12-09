using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt;

public class InvoiceService : IInvoiceService
{
    private readonly ILogger<InvoiceService> _logger;
    private readonly InFaktClient _client;
    private readonly ContractorService _contractorService;

    public InvoiceService(
        ILogger<InvoiceService> logger,
        InFaktClient client,
        ContractorService contractorService)
    {
        _logger = logger;
        _client = client;
        _contractorService = contractorService;
    }

    public async Task<string> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating invoice for contractor {ContractorName} with NIP {NIP}",
            invoice.Contractor.Name, invoice.Contractor.NIP);

        await UpsertContractorAsync(invoice.Contractor, cancellationToken);

        var invoiceResponse = await CreateInvoiceInternalAsync(invoice, cancellationToken);

        _logger.LogInformation("Invoice created successfully for contractor {ContractorName} with number {InvoiceNumber}",
            invoice.Contractor.Name, invoiceResponse.Number);

        return invoiceResponse.Number;
    }

    private async Task UpsertContractorAsync(Contractor contractor, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Preparing contractor: {ContractorName}", contractor.Name);

        try
        {
            var existingContractor = await _contractorService.FindContractorByNIPAsync(contractor.NIP, cancellationToken);

            bool contractorSuccess;
            if (existingContractor != null)
                contractorSuccess = await _contractorService.UpdateContractorAsync(existingContractor.Id, contractor, cancellationToken);
            else
                contractorSuccess = await _contractorService.CreateContractorAsync(contractor, cancellationToken);

            if (!contractorSuccess)
                throw new InvalidOperationException("Failed to create or update contractor in InFakt");

            _logger.LogInformation("Contractor prepared successfully: {ContractorName}", contractor.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating or updating contractor: {ContractorName}", contractor.Name);
            throw new InvalidOperationException("Failed to create or update contractor in InFakt", ex);
        }
    }

    private async Task<Models.InvoiceResponseDto> CreateInvoiceInternalAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        var invoiceRequest = invoice.ToInvoiceDto();
        var invoiceResponse = await _client.CreateInvoiceAsync(invoiceRequest, cancellationToken);

        if (string.IsNullOrEmpty(invoiceResponse.Number))
            throw new InvalidOperationException("Invoice creation failed - no invoice number returned");

        return invoiceResponse;
    }
}