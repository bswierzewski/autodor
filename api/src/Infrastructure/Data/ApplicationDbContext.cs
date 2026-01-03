using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<ExcludedOrder> ExcludedOrders => Set<ExcludedOrder>();
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<ExcludedOrderPosition> ExcludedOrderPositions => Set<ExcludedOrderPosition>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}