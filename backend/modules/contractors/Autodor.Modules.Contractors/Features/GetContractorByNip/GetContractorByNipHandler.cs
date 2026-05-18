using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Contracts.Queries;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Features.GetContractorByNip;

public static class GetContractorByNipHandler
{
    public static async Task<ContractorDto?> Handle(
        GetContractorByNipQuery query, 
        ContractorsDbContext dbContext
        )
    {
        if (string.IsNullOrWhiteSpace(query.Nip))
            return null;

        return await dbContext.Contractors
            .AsNoTracking()
            .Where(c => new TaxId(query.Nip) == c.NIP)
            .Select(c => new ContractorDto(
                c.Id.Value,
                c.Name,
                c.NIP.Value,
                c.Address.Street,
                c.Address.City,
                c.Address.ZipCode,
                c.Email.Value
            ))
            .FirstOrDefaultAsync();
    }
}
