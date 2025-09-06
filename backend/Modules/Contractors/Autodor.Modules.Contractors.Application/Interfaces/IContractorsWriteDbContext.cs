using Autodor.Modules.Contractors.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Interfaces;

public interface IContractorsWriteDbContext
{
    DbSet<Contractor> Contractors { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}