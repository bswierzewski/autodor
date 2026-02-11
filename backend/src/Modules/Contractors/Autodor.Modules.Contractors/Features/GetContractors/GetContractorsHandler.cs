using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Autodor.Modules.Contractors.Features.GetContractors;

public class GetContractorsHandler
{
    [WolverineGet("/contractors")]
    [Tags("Contractors")]
    [EndpointName("GetContractors")]
    [EndpointSummary("Get all contractors, optionally filtered by NIPs")]
    public static async Task<List<GetContractorsResponse>> Handle(
        string[]? nips,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var queryable = dbContext.Contractors.AsNoTracking();

        // Filter by NIPs if provided
        if (nips != null && nips.Length > 0)
        {
            var taxIds = nips.Select(nip => new TaxId(nip)).ToList();
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
