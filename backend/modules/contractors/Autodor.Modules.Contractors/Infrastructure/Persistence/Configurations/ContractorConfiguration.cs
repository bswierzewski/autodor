using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence.Configurations;

public class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
{
    public void Configure(EntityTypeBuilder<Contractor> builder)
    {
        builder.ToTable("Contractors");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => new ContractorId(value))
            .ValueGeneratedNever();

        builder.Property(c => c.NIP)
            .HasConversion(
                nip => nip.Value,
                value => new TaxId(value))
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.NIP)
            .IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(c => c.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200);

            addressBuilder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            addressBuilder.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(20);
        });
    }
}
