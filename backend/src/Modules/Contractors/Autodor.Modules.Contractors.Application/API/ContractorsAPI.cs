using Autodor.Shared.Contracts.Contractors;
using Autodor.Shared.Contracts.Contractors.Dtos;
using Autodor.Modules.Contractors.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.API;

public class ContractorsAPI(IContractorsDbContext dbContext) : IContractorsAPI
{
    public async Task<ContractorDto?> GetContractorByIdAsync(Guid contractorId, CancellationToken cancellationToken = default)
    {
        var contractor = await dbContext.Contractors
            .AsNoTracking()
            .Where(c => c.Id.Value == contractorId)
            .FirstOrDefaultAsync(cancellationToken);

        return contractor?.ToDto();
    }

    public async Task<IEnumerable<ContractorDto>> GetContractorsByNIPsAsync(IEnumerable<string> nips, CancellationToken cancellationToken = default)
    {
        var nipList = nips.ToList();

        var contractors = await dbContext.Contractors
            .AsNoTracking()
            .Where(c => nipList.Contains(c.NIP.Value))
            .ToListAsync(cancellationToken);

        return contractors.Select(c => c.ToDto());
    }

}