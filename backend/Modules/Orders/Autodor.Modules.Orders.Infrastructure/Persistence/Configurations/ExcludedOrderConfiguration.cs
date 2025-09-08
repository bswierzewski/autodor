using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autodor.Modules.Orders.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the ExcludedOrder aggregate.
/// This configuration defines the database schema, constraints, and indexes
/// required for proper persistence of order exclusion records.
/// </summary>
public class ExcludedOrderConfiguration : IEntityTypeConfiguration<ExcludedOrder>
{
    /// <summary>
    /// Configures the ExcludedOrder entity mapping for Entity Framework.
    /// Sets up primary key, property constraints, and unique indexes to ensure data integrity.
    /// </summary>
    /// <param name="builder">The entity type builder for configuring ExcludedOrder</param>
    public void Configure(EntityTypeBuilder<ExcludedOrder> builder)
    {
        // Configure primary key using the inherited Id property from AggregateRoot
        // This ensures each exclusion record has a unique identifier
        builder.HasKey(x => x.Id);

        // Configure OrderId as required with maximum length constraint
        // This prevents data truncation and ensures referential integrity with external systems
        builder.Property(x => x.OrderId)
            .IsRequired()
            .HasMaxLength(50);

        // Configure DateTime as required for audit trail purposes
        // This ensures every exclusion has a timestamp for business compliance
        builder.Property(x => x.DateTime)
            .IsRequired();

        // Create unique index on OrderId to prevent duplicate exclusions
        // This enforces the business rule that an order can only be excluded once
        builder.HasIndex(x => x.OrderId).IsUnique();
    }
}