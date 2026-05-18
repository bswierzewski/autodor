using BuildingBlocks.Tests.Integration.Fixtures;

namespace Autodor.Tests.Integration.Shared;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SharedCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "Shared";
}
