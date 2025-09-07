using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

public class OrdersDbContext : DbContext, IOrdersWriteDbContext, IOrdersReadDbContext
{
    public DbSet<ExcludedOrder> ExcludedOrders { get; set; } = null!;

    IQueryable<ExcludedOrder> IOrdersReadDbContext.ExcludedOrders => ExcludedOrders.AsNoTracking();

    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
        // Interceptory są automatycznie rejestrowane przez AddInterceptors w DI
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExcludedOrderConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}