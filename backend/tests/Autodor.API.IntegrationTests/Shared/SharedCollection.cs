using BuildingBlocks.IntegrationTests.Fixtures;

namespace Autodor.API.IntegrationTests.Shared;

/// <summary>
/// xUnit Collection Fixture that ensures a single DatabaseFixture (PostgreSQL container)
/// is shared across all test classes in the Shared collection.
///
/// Usage:
/// [Collection(SharedCollection.Name)]
/// public class MyTests : BuildingBlocks.IntegrationTests.TestBase&lt;Program&gt;
/// {
///     public MyTests(DatabaseFixture fixture) : base(fixture) { }
/// }
/// </summary>
[CollectionDefinition(Name)]
public class SharedCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "Shared";
}
