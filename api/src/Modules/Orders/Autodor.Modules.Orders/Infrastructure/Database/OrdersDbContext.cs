using Autodor.Modules.Orders.Domain;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Infrastructure.Database;

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Module.Name.ToLowerInvariant());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);

        modelBuilder.MapWolverineEnvelopeStorage();

        base.OnModelCreating(modelBuilder);
    }
}
