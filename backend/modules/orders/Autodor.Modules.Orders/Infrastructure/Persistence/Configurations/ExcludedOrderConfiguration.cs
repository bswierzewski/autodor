using Autodor.Modules.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autodor.Modules.Orders.Infrastructure.Persistence.Configurations;

public class ExcludedOrderConfiguration : IEntityTypeConfiguration<ExcludedOrder>
{
    public void Configure(EntityTypeBuilder<ExcludedOrder> builder)
    {
        builder.ToTable("ExcludedOrders");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(50);
    }
}
