using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ExcludedOrderConfiguration : IEntityTypeConfiguration<ExcludedOrder>
{
    public void Configure(EntityTypeBuilder<ExcludedOrder> builder) { }
}
