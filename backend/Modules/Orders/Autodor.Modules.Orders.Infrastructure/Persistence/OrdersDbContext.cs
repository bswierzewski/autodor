using Autodor.Modules.Orders.Domain.Aggregates;
using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Orders.Infrastructure.Persistence;

/// <summary>
/// Entity Framework database context for the Orders module.
/// This context implements both read and write interfaces to support CQRS pattern
/// and provides access to order-related data persistence operations.
/// </summary>
public class OrdersDbContext : DbContext, IOrdersWriteDbContext, IOrdersReadDbContext
{
    /// <summary>
    /// Gets or sets the DbSet for excluded order entities.
    /// Used for managing orders that have been excluded from processing.
    /// </summary>
    public DbSet<ExcludedOrder> ExcludedOrders { get; set; } = null!;

    /// <summary>
    /// Provides read-only access to excluded orders with no tracking for performance.
    /// This implementation supports the CQRS pattern by optimizing read operations.
    /// </summary>
    IQueryable<ExcludedOrder> IOrdersReadDbContext.ExcludedOrders => ExcludedOrders.AsNoTracking();

    /// <summary>
    /// Initializes a new instance of the OrdersDbContext.
    /// Sets up Entity Framework configuration and interceptor registration.
    /// </summary>
    /// <param name="options">Entity Framework context options for configuration</param>
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
        // Interceptors are automatically registered through AddInterceptors in DI container
        // This enables cross-cutting concerns like auditing and domain event handling
    }

    /// <summary>
    /// Configures the database model and entity mappings.
    /// This method applies all entity configurations to establish the database schema.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity mappings</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply entity-specific configurations to define database schema and constraints
        // This ensures proper mapping between domain entities and database tables
        modelBuilder.ApplyConfiguration(new ExcludedOrderConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}