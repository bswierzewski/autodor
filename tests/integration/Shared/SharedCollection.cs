namespace Autodor.Tests.Integration.Shared;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SharedCollection : ICollectionFixture<AutodorDatabaseFixture>
{
    public const string Name = "Shared";
}
