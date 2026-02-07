using Autodor.Modules.Contractors.Contracts.Abstractions;
using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Infrastructure.Services.ModuleApi;

/// <summary>
/// Internal implementation of Contractors Module API
/// Provides data access to other modules
/// </summary>
public class ContractorsModuleApi(ContractorsDbContext dbContext) : IContractorsModuleApi
{
    public async Task<ContractorDto?> GetContractorByNipAsync(string nip, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nip))
            return null;

        return await dbContext.Contractors
            .AsNoTracking()
            .Where(c => new TaxId(nip) == c.NIP)
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
