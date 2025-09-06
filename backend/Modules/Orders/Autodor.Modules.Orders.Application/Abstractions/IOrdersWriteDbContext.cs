using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Application.Abstractions;

public interface IOrdersWriteDbContext
{
    DbSet<ExcludedOrder> ExcludedOrders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}