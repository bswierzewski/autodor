using Autodor.Modules.Products.Domain.ValueObjects;
using Autodor.Modules.Products.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Products.Infrastructure.Persistence;

public class ProductsDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;

    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
        // Interceptory są automatycznie rejestrowane przez AddInterceptors w DI
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}