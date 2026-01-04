using Autodor.Shared.Contracts.Contractors.Dtos;

namespace Autodor.Shared.Contracts.Contractors;

public interface IContractorsAPI
{
    Task<ContractorDto?> GetContractorByIdAsync(Guid contractorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContractorDto>> GetContractorsByNIPsAsync(IEnumerable<string> nips, CancellationToken cancellationToken = default);
}