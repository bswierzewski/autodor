using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Infrastructure.Persistence;

/// <summary>
/// Entity Framework database context for the Contractors module in the automotive parts distribution system.
/// This context implements both read and write interfaces to support the CQRS pattern while maintaining
/// a single database connection. Provides optimized access patterns for both command and query operations
/// while ensuring proper entity configuration and database schema management.
/// </summary>
public class ContractorsDbContext : DbContext, IContractorsWriteDbContext, IContractorsReadDbContext
{
    /// <summary>
    /// Gets or sets the DbSet for contractor entities, enabling full Entity Framework operations.
    /// This property supports create, read, update, and delete operations with change tracking,
    /// relationship management, and transaction support for contractor data persistence.
    /// </summary>
    public DbSet<Contractor> Contractors { get; set; }

    /// <summary>
    /// Provides read-only access to contractors without change tracking for optimal query performance.
    /// This explicit interface implementation supports the CQRS pattern by offering a no-tracking
    /// queryable collection specifically designed for read operations and reporting scenarios.
    /// </summary>
    IQueryable<Contractor> IContractorsReadDbContext.Contractors => Contractors.AsNoTracking();

    /// <summary>
    /// Initializes a new instance of the ContractorsDbContext with the specified options.
    /// The context is configured through dependency injection with database connection details,
    /// interceptors, and other Entity Framework configurations.
    /// </summary>
    /// <param name="options">The database context options including connection strings and configuration</param>
    public ContractorsDbContext(DbContextOptions<ContractorsDbContext> options) : base(options)
    {
        // Interceptors are automatically registered through AddInterceptors in dependency injection
        // This enables features like audit logging, soft delete, and performance monitoring
    }

    /// <summary>
    /// Configures the database model and entity relationships for the Contractors module.
    /// This method applies entity configurations, defines database schema, sets up value object conversions,
    /// and establishes constraints to ensure data integrity and optimal database performance.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the database schema</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply contractor-specific entity configuration
        // This includes value object conversions, constraints, and indexing strategies
        modelBuilder.ApplyConfiguration(new ContractorConfiguration());
        
        // Ensure base configurations are applied for inherited behaviors
        // This maintains compatibility with any base context configurations
        base.OnModelCreating(modelBuilder);
    }
}