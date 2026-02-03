using Autodor.API.IntegrationTests.Shared;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;

namespace Autodor.API.IntegrationTests.Modules.Orders.Products;

[Collection(SharedCollection.Name)]
public class ProductsServiceTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{

}
