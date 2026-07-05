using System.Net;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Extensions;
using BuildingBlocks.Tests.Integration.Fixtures;

namespace Autodor.Tests.Integration.Orders.GetOrders;

public class GetOrdersTests(AutodorDatabaseFixture databaseFixture, HostFixture<Program> hostFixture, ITestOutputHelper output) : IntegrationTest(databaseFixture, hostFixture)
{
    [Fact(Skip = "Manual test - requires real API connection and valid date range")]
    public async Task GetOrders_WhenOrdersExist_ShouldReturnOk()
    {
        // Arrange - Replace with actual date range from test environment
        var from = new DateTime(2026, 2, 5);
        var to = new DateTime(2026, 2, 7);

        // Act && Assert
        var response = await Host.Scenario(s =>
        {
            s.Get.Url($"/api/orders?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output);
    }

    [Fact(Skip = "Disabled by default")]
    public async Task GetOrders_WhenFromDateIsMissing_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var to = new DateTime(2026, 2, 7);

        // Act && Assert
        await Host.Scenario(s =>
        {
            s.Get.Url($"/api/orders?to={to:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }

    [Fact(Skip = "Disabled by default")]
    public async Task GetOrders_WhenToDateIsMissing_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var from = new DateTime(2026, 2, 5);

        // Act && Assert
        await Host.Scenario(s =>
        {
            s.Get.Url($"/api/orders?from={from:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }

    [Fact(Skip = "Disabled by default")]
    public async Task GetOrders_WhenToDateIsBeforeFromDate_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var from = new DateTime(2026, 2, 7);
        var to = new DateTime(2026, 2, 5);

        // Act && Assert
        await Host.Scenario(s =>
        {
            s.Get.Url($"/api/orders?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }

    [Fact(Skip = "Disabled by default")]
    public async Task GetOrders_WhenBothDatesAreMissing_ShouldReturnValidationProblemDetails()
    {
        // Act && Assert
        await Host.Scenario(s =>
        {
            s.Get.Url("/api/orders");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        }).PrintBody(output);
    }
}
