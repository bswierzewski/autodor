using Autodor.Modules.Products.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autodor.Modules.Products.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the BackgroundTaskState entity, defining database schema constraints and indexes.
/// </summary>
public class BackgroundTaskStateConfiguration : IEntityTypeConfiguration<BackgroundTaskState>
{
    /// <summary>
    /// Configures the BackgroundTaskState entity with primary key, required fields, unique constraints and indexes.
    /// </summary>
    /// <param name="builder">The entity type builder for BackgroundTaskState configuration</param>
    public void Configure(EntityTypeBuilder<BackgroundTaskState> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TaskName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.LastExecutedAt)
            .IsRequired();

        builder.Property(x => x.AdditionalData)
            .IsRequired(false)
            .HasMaxLength(4000);

        builder.HasIndex(x => x.TaskName)
            .IsUnique()
            .HasDatabaseName("IX_BackgroundTaskStates_TaskName");
    }
}