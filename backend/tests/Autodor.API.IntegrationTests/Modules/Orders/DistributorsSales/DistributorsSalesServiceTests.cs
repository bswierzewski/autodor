using Autodor.API.IntegrationTests.Shared;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;

namespace Autodor.API.IntegrationTests.Modules.Orders.DistributorsSales;

[Collection(SharedCollection.Name)]
public class DistributorsSalesServiceTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
}
