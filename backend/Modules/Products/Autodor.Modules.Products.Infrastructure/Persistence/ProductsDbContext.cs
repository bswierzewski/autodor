using Autodor.Modules.Products.Domain.Aggregates;
using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Infrastructure.Persistence;

/// <summary>
/// Entity Framework database context for the Products module.
/// Implements both read and write interfaces to support CQRS pattern separation of concerns.
/// </summary>
public class ProductsDbContext : DbContext, IProductsWriteDbContext, IProductsReadDbContext
{
    /// <summary>
    /// Gets or sets the Products database set for entity operations.
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;
    
    /// <summary>
    /// Provides read-only access to Products with change tracking disabled for better performance.
    /// This implementation supports the CQRS read-side operations.
    /// </summary>
    IQueryable<Product> IProductsReadDbContext.Products => Products.AsNoTracking();

    /// <summary>
    /// Initializes a new instance of the ProductsDbContext with specified options.
    /// </summary>
    /// <param name="options">Database context configuration options including connection strings and providers</param>
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
        // Interceptors are automatically registered through AddInterceptors in DI container
        // This enables cross-cutting concerns like auditing, domain events, etc.
    }

    /// <summary>
    /// Configures the entity model using Fluent API configurations.
    /// Applies domain-specific entity configurations for proper database mapping.
    /// </summary>
    /// <param name="modelBuilder">Builder used to construct the entity model</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply entity-specific configuration to ensure proper database schema
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}