namespace Autodor.Tests.Integration.Shared;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SharedCollection : ICollectionFixture<SharedEnvironment>
{
    public const string Name = "Shared";
}
