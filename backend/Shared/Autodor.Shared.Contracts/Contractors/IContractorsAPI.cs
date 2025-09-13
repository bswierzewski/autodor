using Autodor.Shared.Contracts.Contractors.Dtos;

namespace Autodor.Shared.Contracts.Contractors;

public interface IContractorsAPI
{
    /// <summary>
    /// Gets contractor details by ID
    /// </summary>
    Task<ContractorDto?> GetContractorByIdAsync(Guid contractorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple contractors by their NIPs (tax identification numbers)
    /// </summary>
    Task<IEnumerable<ContractorDto>> GetContractorsByNIPsAsync(IEnumerable<string> nips, CancellationToken cancellationToken = default);
}