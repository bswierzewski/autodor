using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the Contractor aggregate root.
/// This configuration class defines the database schema, value object conversions, constraints,
/// and indexing strategies for optimal contractor data storage and retrieval.
/// Implements domain-driven design principles by properly mapping domain concepts to database structures.
/// </summary>
public class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
{
    /// <summary>
    /// Configures the Contractor entity mapping for Entity Framework.
    /// This method defines how the domain aggregate is persisted to the database,
    /// including value object conversions, constraints, and performance optimizations.
    /// Ensures data integrity while supporting efficient business operations.
    /// </summary>
    /// <param name="builder">The entity type builder for configuring the Contractor entity</param>
    public void Configure(EntityTypeBuilder<Contractor> builder)
    {
        // Configure the primary key using the strongly-typed ContractorId
        // This ensures type safety and prevents primitive obsession
        builder.HasKey(x => x.Id);

        // Configure ContractorId value object conversion for database storage
        // Stores the GUID value while maintaining domain type safety in the application
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ContractorId(x));

        // Configure TaxId (NIP) value object with business constraints
        // Required field with unique constraint for legal compliance and business rules
        builder.Property(x => x.NIP)
            .HasConversion(x => x.Value, x => new TaxId(x))  // Value object conversion
            .IsRequired()                                     // Business requirement
            .HasMaxLength(20);                               // Polish NIP format constraint

        // Configure business name with appropriate length constraints
        // Essential for contractor identification and legal documentation
        builder.Property(x => x.Name)
            .IsRequired()                                     // Business requirement
            .HasMaxLength(200);                              // Reasonable business name length

        // Configure Address value object as a complex property
        // This approach keeps related address data together while maintaining value object semantics
        builder.ComplexProperty(x => x.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200);   // Street address constraint
            address.Property(a => a.City).HasMaxLength(100);     // City name constraint
            address.Property(a => a.ZipCode).HasMaxLength(20);   // Postal code constraint
        });

        // Configure Email value object with business constraints
        // Essential for business communication and notifications
        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, x => new Email(x))  // Value object conversion
            .IsRequired()                                     // Business requirement
            .HasMaxLength(300);                              // Standard email length

        // Create unique index on NIP for business rule enforcement
        // Ensures no duplicate tax identification numbers in the system
        // Critical for legal compliance and business integrity
        builder.HasIndex(x => x.NIP).IsUnique();
    }
}