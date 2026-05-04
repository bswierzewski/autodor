using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Extensions;
using System.Net;

namespace Autodor.Tests.Integration.Modules.Orders.GetOrder;

[Collection(SharedCollection.Name)]
public class GetOrderTests(SharedEnvironment Environment, ITestOutputHelper output) : IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        await Environment.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact(Skip = "Manual test - requires real API connection and valid order ID")]
    public async Task GetOrder_WhenOrderExists_ShouldReturnOk()
    {
        // Arrange - Replace with actual order ID and date from test environment
        var orderId = "3ff0615c-b902-f111-95f5-00155d0b7aef";
        var orderDate = new DateTime(2026, 2, 5);

        // Act && Assert
        var response = await Environment.Host.Scenario(s =>
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
        await Environment.Host.Scenario(s =>
        {
            s.Get.Url($"/orders/{orderId}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }
}