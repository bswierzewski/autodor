using Autodor.Modules.Products.Domain.Aggregates;
using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Infrastructure.Persistence;

/// <summary>
/// Entity Framework database context for the Products module, providing both read and write access to product data.
/// </summary>
public class ProductsDbContext : DbContext, IProductsWriteDbContext, IProductsReadDbContext
{
    /// <summary>
    /// Gets or sets the Products DbSet for write operations.
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;
    
    /// <summary>
    /// Provides read-only access to products with change tracking disabled for performance.
    /// </summary>
    IQueryable<Product> IProductsReadDbContext.Products => Products.AsNoTracking();

    /// <summary>
    /// Initializes a new instance of the ProductsDbContext class.
    /// </summary>
    /// <param name="options">Database context options for configuration</param>
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the database schema and applies entity configurations.
    /// </summary>
    /// <param name="modelBuilder">The model builder for entity configuration</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}