using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Contractors.Features.GetContractors;

public class GetContractorsHandler
{
    [WolverineGet("/contractors")]
    [Tags("Contractors")]
    public static async Task<List<GetContractorsResponse>> Handle(
        [AsParameters] GetContractorsQuery query,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var queryable = dbContext.Contractors.AsNoTracking();

        // Filter by NIPs if provided
        if (query.NIPs != null && query.NIPs.Length > 0)
        {
            var taxIds = query.NIPs.Select(nip => new TaxId(nip)).ToList();
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
