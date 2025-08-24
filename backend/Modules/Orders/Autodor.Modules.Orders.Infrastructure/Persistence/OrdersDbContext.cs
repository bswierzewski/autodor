using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

public class OrdersDbContext : DbContext
{
    public DbSet<ExcludedOrder> ExcludedOrders { get; set; } = null!;

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