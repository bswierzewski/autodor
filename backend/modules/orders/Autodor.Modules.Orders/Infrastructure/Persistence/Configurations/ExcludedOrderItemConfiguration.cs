using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autodor.Modules.Orders.Infrastructure.Persistence.Configurations;

public class ExcludedOrderItemConfiguration : IEntityTypeConfiguration<ExcludedOrderItem>
{
    public void Configure(EntityTypeBuilder<ExcludedOrderItem> builder)
    {
        builder.ToTable("ExcludedOrderItems");

        // Configure composite key
        builder.HasKey(e => new { e.OrderId, e.ItemNumber });

        builder.Property(e => e.OrderId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ItemNumber)
            .IsRequired()
            .HasMaxLength(50);
    }
}
