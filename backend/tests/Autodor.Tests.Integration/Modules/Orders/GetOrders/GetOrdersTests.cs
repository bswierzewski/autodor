using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Extensions;
using System.Net;

namespace Autodor.Tests.Integration.Modules.Orders.GetOrders;

[Collection(SharedCollection.Name)]
public class GetOrdersTests(SharedEnvironment Environment, ITestOutputHelper output) : IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        await Environment.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact(Skip = "Manual test - requires real API connection and valid date range")]
    public async Task GetOrders_WhenOrdersExist_ShouldReturnOk()
    {
        // Arrange - Replace with actual date range from test environment
        var from = new DateTime(2026, 2, 5);
        var to = new DateTime(2026, 2, 7);

        // Act && Assert
        var response = await Environment.Host.Scenario(s =>
        {
            s.Get.Url($"/orders?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output);
    }

    [Fact]
    public async Task GetOrders_WhenFromDateIsMissing_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var to = new DateTime(2026, 2, 7);

        // Act && Assert
        await Environment.Host.Scenario(s =>
        {
            s.Get.Url($"/orders?to={to:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }

    [Fact]
    public async Task GetOrders_WhenToDateIsMissing_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var from = new DateTime(2026, 2, 5);

        // Act && Assert
        await Environment.Host.Scenario(s =>
        {
            s.Get.Url($"/orders?from={from:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }

    [Fact]
    public async Task GetOrders_WhenToDateIsBeforeFromDate_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var from = new DateTime(2026, 2, 7);
        var to = new DateTime(2026, 2, 5);

        // Act && Assert
        await Environment.Host.Scenario(s =>
        {
            s.Get.Url($"/orders?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }

    [Fact]
    public async Task GetOrders_WhenBothDatesAreMissing_ShouldReturnValidationProblemDetails()
    {
        // Act && Assert
        await Environment.Host.Scenario(s =>
        {
            s.Get.Url("/orders");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }
}
