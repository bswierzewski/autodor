using BuildingBlocks.Tests.Integration.Fixtures;
using Respawn.Graph;

namespace Autodor.Tests.Integration.Shared;

public sealed class AutodorDatabaseFixture : DatabaseFixture
{
    protected override string[] SchemasToInclude => ["contractors", "orders", "wolverine"];

    protected override Table[] TablesToIgnore =>
    [
        new("contractors", "__EFMigrationsHistory"),
        new("orders", "__EFMigrationsHistory")
    ];
}
