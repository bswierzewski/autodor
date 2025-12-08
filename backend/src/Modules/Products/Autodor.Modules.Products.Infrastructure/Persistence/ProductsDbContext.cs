using Autodor.Modules.Products.Application.Abstractions;
using Autodor.Modules.Products.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Infrastructure.Persistence;

/// <summary>
/// Entity Framework database context for the Products module.
/// </summary>
public class ProductsDbContext : DbContext, IProductsDbContext
{
    /// <summary>
    /// Gets or sets the Products DbSet.
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// Gets or sets the BackgroundTaskState DbSet for background task state management.
    /// </summary>
    public DbSet<BackgroundTaskState> BackgroundTaskStates { get; set; } = null!;

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
        // Apply entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}