using System.Net;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Extensions;
using BuildingBlocks.Tests.Integration.Fixtures;

namespace Autodor.Tests.Integration.Orders.GetOrder;

public class GetOrderTests(AutodorDatabaseFixture databaseFixture, HostFixture<Program> hostFixture, ITestOutputHelper output) : IntegrationTest(databaseFixture, hostFixture)
{
    [Fact(Skip = "Manual test - requires real API connection and valid order ID")]
    public async Task GetOrder_WhenOrderExists_ShouldReturnOk()
    {
        // Arrange - Replace with actual order ID and date from test environment
        var orderId = "3ff0615c-b902-f111-95f5-00155d0b7aef";
        var orderDate = new DateTime(2026, 2, 5);

        // Act && Assert
        var response = await Host.Scenario(s =>
        {
            s.Get.Url($"/api/orders/{orderId}?date={orderDate:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output);
    }

    [Fact(Skip = "Disabled by default")]
    public async Task GetOrder_WhenDateIsMissing_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var orderId = "TEST-ORDER-ID";

        // Act && Assert
        await Host.Scenario(s =>
        {
            s.Get.Url($"/api/orders/{orderId}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }
}
