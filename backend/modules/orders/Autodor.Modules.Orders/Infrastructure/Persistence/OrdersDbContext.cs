using Autodor.Modules.Orders.Domain.Aggregates;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

public sealed class Factory : ModuleDbContextDesignTimeFactory<OrdersDbContext> { }

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options)
    : ModuleDbContext<OrdersDbContext>(options, Schema)
{
    public const string Schema = "orders";

    public DbSet<ExcludedOrder> ExcludedOrders => Set<ExcludedOrder>();
    public DbSet<ExcludedOrderItem> ExcludedOrderItems => Set<ExcludedOrderItem>();
}
