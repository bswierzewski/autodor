using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

/// <summary>
/// Database context for the Orders module, providing access to order-related entities.
/// Implements both read and write context interfaces for CQRS pattern support.
/// </summary>
public class OrdersDbContext : DbContext, IOrdersWriteDbContext, IOrdersReadDbContext
{
    /// <summary>
    /// Gets or sets the DbSet for excluded orders.
    /// </summary>
    public DbSet<ExcludedOrder> ExcludedOrders { get; set; } = null!;

    /// <summary>
    /// Provides read-only access to excluded orders with no tracking for performance optimization.
    /// </summary>
    IQueryable<ExcludedOrder> IOrdersReadDbContext.ExcludedOrders => ExcludedOrders.AsNoTracking();

    /// <summary>
    /// Initializes a new instance of the OrdersDbContext.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model and relationships for the Orders module entities.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}