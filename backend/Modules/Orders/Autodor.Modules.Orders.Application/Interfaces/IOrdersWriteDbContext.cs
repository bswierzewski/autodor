using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Application.Interfaces;

public interface IOrdersWriteDbContext
{
    DbSet<ExcludedOrder> ExcludedOrders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}