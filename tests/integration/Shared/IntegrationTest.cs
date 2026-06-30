using BuildingBlocks.Tests.Integration.Fixtures;

namespace Autodor.Tests.Integration.Shared;

/// <summary>
/// Base class for Autodor integration tests that share the application database collection and use the API entry point.
/// </summary>
/// <param name="databaseFixture">Shared database fixture used by the integration-test collection.</param>
/// <param name="hostFixture">Application host fixture owned by the current test class.</param>
[Collection(SharedCollection.Name)]
public abstract class IntegrationTest(AutodorDatabaseFixture databaseFixture, HostFixture<Program> hostFixture)
    : BuildingBlocks.Tests.Integration.IntegrationTestBase<Program, AutodorDatabaseFixture>(databaseFixture, hostFixture);
