using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Features.GetContractorByNIP;

public static class GetContractorByNIPHandler
{
    public static async Task<ContractorDto?> Handle(
        GetContractorByNIPQuery query,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query.NIP))
            return null;

        return await dbContext.Contractors
            .AsNoTracking()
            .Where(c => new TaxId(query.NIP) == c.NIP)
            .Select(c => new ContractorDto(
                c.Id.Value,
                c.Name,
                c.NIP.Value,
                c.Address.Street,
                c.Address.City,
                c.Address.ZipCode,
                c.Email.Value
            ))
            .FirstOrDefaultAsync(ct);
    }
}
