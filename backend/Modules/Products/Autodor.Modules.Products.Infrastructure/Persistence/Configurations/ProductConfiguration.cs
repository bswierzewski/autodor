using Autodor.Modules.Products.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autodor.Modules.Products.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the Product entity, defining database schema constraints and relationships.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Configures the Product entity with primary key, required fields, and length constraints.
    /// </summary>
    /// <param name="builder">The entity type builder for Product configuration</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Number)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EAN13)
            .IsRequired()
            .HasMaxLength(20);
    }
}