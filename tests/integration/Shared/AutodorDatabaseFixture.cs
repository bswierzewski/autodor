using BuildingBlocks.Infrastructure.Modules.Extensions;
using BuildingBlocks.Tests.Integration.Fixtures;
using Respawn.Graph;

namespace Autodor.Tests.Integration.Shared;

/// <summary>
/// Shared PostgreSQL fixture that configures migrations and database cleanup for Autodor integration tests.
/// </summary>
public sealed class AutodorDatabaseFixture : DatabaseFixture
{
    /// <summary>
    /// Application schemas whose data is reset between integration tests.
    /// </summary>
    protected override string[] SchemasToInclude => ["contractors", "orders"];

    /// <summary>
    /// Module migration history tables preserved during database resets.
    /// </summary>
    protected override Table[] TablesToIgnore =>
    [
        new("contractors", "__EFMigrationsHistory"),
        new("orders", "__EFMigrationsHistory")
    ];

    /// <summary>
    /// Applies migrations for all application modules using the integration-test host services.
    /// </summary>
    /// <param name="services">Service provider from the running application host.</param>
    protected override Task MigrateAsync(IServiceProvider services) =>
        services.ApplyModuleMigrationsAsync();
}
