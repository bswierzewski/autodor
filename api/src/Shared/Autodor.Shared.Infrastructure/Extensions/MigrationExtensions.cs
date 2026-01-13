using Autodor.Shared.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autodor.Shared.Infrastructure.Extensions;

public static class MigrationExtensions
{
    /// <summary>
    /// Executes EF Core migrations for the specified DbContext.
    /// </summary>
    public static async Task MigrateDatabaseAsync<TContext>(this IHost host, CancellationToken cancellationToken = default)
        where TContext : DbContext
    {        
        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        var logger = host.Services.GetRequiredService<ILogger<MigrationService<TContext>>>();

        var migrationService = new MigrationService<TContext>(scopeFactory, logger);

        await migrationService.MigrateAsync(cancellationToken);
    }
}