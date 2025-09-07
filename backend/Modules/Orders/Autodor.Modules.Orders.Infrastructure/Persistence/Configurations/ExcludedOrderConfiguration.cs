using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autodor.Modules.Orders.Infrastructure.Persistence.Configurations;

public class ExcludedOrderConfiguration : IEntityTypeConfiguration<ExcludedOrder>
{
    public void Configure(EntityTypeBuilder<ExcludedOrder> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DateTime)
            .IsRequired();

        builder.HasIndex(x => x.Number).IsUnique();
    }
}