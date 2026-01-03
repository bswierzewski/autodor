using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ExcludedOrder> ExcludedOrders { get; }
    DbSet<Contractor> Contractors { get; }
    DbSet<ExcludedOrderPosition> ExcludedOrderPositions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
