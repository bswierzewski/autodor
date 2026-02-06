using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Orders.Features.GetOrder;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;
using System.Net;
using Xunit.Abstractions;

namespace Autodor.API.IntegrationTests.Modules.Orders.GetOrder;

[Collection(SharedCollection.Name)]
public class GetOrderTests(DatabaseFixture databaseFixture, ITestOutputHelper output) : TestBase<Program>(databaseFixture)
{
    [Fact(Skip = "Manual test - requires real API connection and valid order ID")]
    public async Task GetOrder_WhenOrderExists_ShouldReturnOk()
    {
        // Arrange - Replace with actual order ID and date from test environment
        var orderId = "3ff0615c-b902-f111-95f5-00155d0b7aef";
        var orderDate = new DateTime(2026, 2, 5);

        // Act && Assert
        var response = await AlbaHost.Scenario(s =>
        {
            s.Get.Url($"/orders/{orderId}?date={orderDate:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output);
    }

    [Fact]
    public async Task GetOrder_WhenDateIsMissing_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var orderId = "TEST-ORDER-ID";

        // Act && Assert
        await AlbaHost.Scenario(s =>
        {
            s.Get.Url($"/orders/{orderId}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }
}