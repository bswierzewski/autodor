using BuildingBlocks.Tests.Infrastructure.Containers;

namespace Autodor.Tests.EndToEnd;

public class AutodorSharedFixture : IAsyncLifetime
{
    public PostgreSqlTestContainer Container { get; } = new();

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => Task.CompletedTask;
}

[CollectionDefinition("Autodor")]
public class AutodorCollection : ICollectionFixture<AutodorSharedFixture>
{
}
