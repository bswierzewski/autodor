using Autodor.Modules.Products.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autodor.Modules.Products.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the Product entity.
/// Defines database schema constraints, relationships, and mappings using Fluent API.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Configures the Product entity mapping to database schema.
    /// Sets up primary keys, column constraints, and business rule validations.
    /// </summary>
    /// <param name="builder">Entity type builder for fluent configuration</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Configure primary key using inherited Id from AggregateRoot
        builder.HasKey(x => x.Id);

        // Configure Name property with business constraints
        // Max length allows for full product descriptions while preventing excessive data
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(500);

        // Configure Number property for part identification
        // Length sufficient for most supplier part numbering schemes
        builder.Property(x => x.Number)
            .IsRequired()
            .HasMaxLength(100);

        // Configure EAN13 property for barcode storage
        // Standard EAN-13 is 13 digits, but allowing extra space for formatting characters
        builder.Property(x => x.EAN13)
            .IsRequired()
            .HasMaxLength(20);
    }
}