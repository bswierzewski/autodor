using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

public class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Database=Autodor", npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", OrdersDbContext.Schema);
        });

        return new OrdersDbContext(optionsBuilder.Options);
    }
}
