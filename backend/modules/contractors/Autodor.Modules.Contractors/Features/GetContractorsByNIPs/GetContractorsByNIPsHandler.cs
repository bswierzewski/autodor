using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Features.GetContractorsByNIPs;

public static class GetContractorsByNIPsHandler
{
    public static async Task<IEnumerable<ContractorDto>> Handle(
        GetContractorsByNIPsQuery query,
        ContractorsDbContext dbContext)
    {
        if (!query.NIPs.Any())
            return [];

        var nips = query.NIPs.Select(n => new TaxId(n)).ToList();

        return await dbContext.Contractors
            .AsNoTracking()
            .Where(c => nips.Contains(c.NIP))
            .Select(c => new ContractorDto(
                c.Id.Value,
                c.Name,
                c.NIP.Value,
                c.Address.Street,
                c.Address.City,
                c.Address.ZipCode,
                c.Email.Value
            ))
            .ToListAsync();
    }
}
