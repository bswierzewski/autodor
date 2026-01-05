using BuildingBlocks.Tests.Infrastructure.Containers;

namespace Autodor.Tests.EndToEnd;

public class AutodorSharedFixture : IAsyncLifetime
{
    public PostgreSqlTestContainer Container { get; } = new();

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }
}

[CollectionDefinition("Autodor")]
public class AutodorCollection : ICollectionFixture<AutodorSharedFixture>
{
}
