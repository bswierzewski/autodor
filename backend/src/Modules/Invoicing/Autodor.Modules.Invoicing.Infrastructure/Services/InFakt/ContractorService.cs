using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Models;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt;

public class ContractorService
{
    private readonly ILogger<ContractorService> _logger;
    private readonly InFaktClient _client;

    public ContractorService(
        ILogger<ContractorService> logger,
        InFaktClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<ContractorResponseDto?> FindContractorByNIPAsync(string nip, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _client.FindContractorByNIPAsync(nip, cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CreateContractorAsync(Contractor contractor, CancellationToken cancellationToken = default)
    {
        try
        {
            var contractorDto = contractor.ToContractorDto();
            var response = await _client.CreateContractorAsync(contractorDto, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var parsedError = ErrorHandler.ParseError(errorContent);
                _logger.LogError("Failed to create contractor: {Error}", parsedError);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contractor: {ContractorName}", contractor.Name);
            return false;
        }
    }

    public async Task<bool> UpdateContractorAsync(int contractorId, Contractor contractor, CancellationToken cancellationToken = default)
    {
        try
        {
            var contractorDto = contractor.ToContractorDto();
            var response = await _client.UpdateContractorAsync(contractorId, contractorDto, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var parsedError = ErrorHandler.ParseError(errorContent);
                _logger.LogError("Failed to update contractor: {Error}", parsedError);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contractor: {ContractorName}", contractor.Name);
            return false;
        }
    }
}