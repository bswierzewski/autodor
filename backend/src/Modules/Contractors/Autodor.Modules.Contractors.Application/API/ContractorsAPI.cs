using Autodor.Shared.Contracts.Contractors;
using Autodor.Shared.Contracts.Contractors.Dtos;
using Autodor.Modules.Contractors.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.API;

/// <summary>
/// Implementation of IContractorsAPI that provides contractor data access for external modules.
/// </summary>
public class ContractorsAPI : IContractorsAPI
{
    private readonly IContractorsDbContext _dbContext;

    public ContractorsAPI(IContractorsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets contractor details by ID from the database.
    /// </summary>
    /// <param name="contractorId">Unique identifier of the contractor</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>Contractor details or null if not found</returns>
    public async Task<ContractorDto?> GetContractorByIdAsync(Guid contractorId, CancellationToken cancellationToken = default)
    {
        var contractor = await _dbContext.Contractors
            .AsNoTracking()
            .Where(c => c.Id.Value == contractorId)
            .FirstOrDefaultAsync(cancellationToken);

        return contractor?.ToDto();
    }

    /// <summary>
    /// Gets multiple contractors by their NIPs (tax identification numbers) from the database.
    /// </summary>
    /// <param name="nips">Collection of NIPs to retrieve contractors for</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>Collection of contractors matching the provided NIPs</returns>
    public async Task<IEnumerable<ContractorDto>> GetContractorsByNIPsAsync(IEnumerable<string> nips, CancellationToken cancellationToken = default)
    {
        var nipList = nips.ToList();

        var contractors = await _dbContext.Contractors
            .AsNoTracking()
            .Where(c => nipList.Contains(c.NIP.Value))
            .ToListAsync(cancellationToken);

        return contractors.Select(c => c.ToDto());
    }

}