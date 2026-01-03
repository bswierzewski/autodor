using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ExcludedOrderPositionConfiguration : IEntityTypeConfiguration<ExcludedOrderPosition>
{
    public void Configure(EntityTypeBuilder<ExcludedOrderPosition> builder)
    {
        builder.HasIndex(e => new { e.OrderId, e.PartNumber })
               .IsUnique();

        builder.Property(e => e.OrderId)
               .IsRequired();

        builder.Property(e => e.PartNumber)
               .IsRequired();
    }
}
