using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Features.GetContractors;

public static class GetContractorsHandler
{
    [Authorize]
    public static async Task<List<GetContractorsResponse>> Handle(
        GetContractorsCommand command,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var queryable = dbContext.Contractors.AsNoTracking();

        // Filter by NIPs if provided
        if (command.NIPs != null && command.NIPs.Length > 0)
        {
            var taxIds = command.NIPs.Select(nip => new TaxId(nip)).ToList();
            queryable = queryable.Where(c => taxIds.Contains(c.NIP));
        }

        return await queryable
            .Select(c => new GetContractorsResponse(
                c.Id.Value,
                c.Name,
                c.NIP.Value,
                c.Address.Street,
                c.Address.City,
                c.Address.ZipCode,
                c.Email.Value
            ))
            .ToListAsync(ct);
    }
}
