using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt;

public class InFaktPreProcessor : IInvoicePreProcessor
{
    private readonly ILogger<InFaktPreProcessor> _logger;
    private readonly InFaktContractorService _contractorService;

    public InFaktPreProcessor(
        ILogger<InFaktPreProcessor> logger,
        InFaktContractorService contractorService)
    {
        _logger = logger;
        _contractorService = contractorService;
    }

    public async Task PrepareInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Preparing InFakt invoice for contractor: {ContractorName}", invoice.Contractor.Name);

        try
        {
            var existingContractor = await _contractorService.FindContractorByNIPAsync(invoice.Contractor.NIP, cancellationToken);

            bool contractorSuccess;
            if (existingContractor != null)
            {
                contractorSuccess = await _contractorService.UpdateContractorAsync(existingContractor.Id, invoice.Contractor, cancellationToken);
            }
            else
            {
                contractorSuccess = await _contractorService.CreateContractorAsync(invoice.Contractor, cancellationToken);
            }

            if (!contractorSuccess)
            {
                throw new InvalidOperationException("Failed to create or update contractor in InFakt");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating or updating contractor: {ContractorName}", invoice.Contractor.Name);
            throw new InvalidOperationException("Failed to create or update contractor in InFakt", ex);
        }

        _logger.LogInformation("Contractor prepared successfully for InFakt invoice: {ContractorName}", invoice.Contractor.Name);
    }
}