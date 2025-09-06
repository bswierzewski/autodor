using Autodor.Modules.Contractors.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Abstractions;

public interface IContractorsWriteDbContext
{
    DbSet<Contractor> Contractors { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}