using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Features.GetContractors;

public class GetContractorsHandler
{
    public static async Task<List<GetContractorsResponse>> Handle(
        GetContractorsCommand command,
        ContractorsDbContext dbContext,
        CancellationToken ct)
    {
        var queryable = dbContext.Contractors.AsNoTracking();

        // Filter by NIPs if provided
        if (command.Nips != null && command.Nips.Length > 0)
        {
            var taxIds = command.Nips.Select(nip => new TaxId(nip)).ToList();
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
