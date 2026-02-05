using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Orders.Features.GetOrder;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Autodor.API.IntegrationTests.Modules.Orders.GetOrder;

[Collection(SharedCollection.Name)]
public class GetOrderTests(DatabaseFixture databaseFixture) : TestBase<Program>(databaseFixture)
{
    [Fact(Skip = "Manual test - requires real API connection and valid order ID")]
    public async Task GetOrder_WhenOrderExists_ShouldReturnOk()
    {
        // Arrange - Replace with actual order ID and date from test environment
        var orderId = "3ff0615c-b902-f111-95f5-00155d0b7aef";
        var orderDate = new DateTime(2026, 2, 5);

        // Act
        var response = await AlbaHost.Scenario(s =>
        {
            s.Get.Url($"/orders/{orderId}?date={orderDate:yyyy-MM-dd}");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        // Assert
        var result = await response.ReadAsJsonAsync<GetOrderResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.Number.Should().NotBeNullOrEmpty();
        result.Items.Should().NotBeNull();
    }

    [Fact(Skip = "Manual test - requires real API connection and valid order ID")]
    public async Task GetOrder_WhenDateIsMissing_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var orderId = "TEST-ORDER-ID";

        // Act
        var response = await AlbaHost.Scenario(s =>
        {
            s.Get.Url($"/orders/{orderId}");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });

        // Assert
        var problemDetails = await response.ReadAsJsonAsync<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();

        // Verify all RFC 7807 ProblemDetails fields are populated
        problemDetails!.Status.Should().Be(400);
        problemDetails.Title.Should().Be("One or more validation errors occurred.");
        problemDetails.Type.Should().Be("https://tools.ietf.org/html/rfc9110#section-15.5.1");
        //problemDetails.Detail.Should().Be("Please correct the validation errors and try again.");
        problemDetails.Instance.Should().Be("/orders/TEST-ORDER-ID");

        // Verify Extensions contain traceId (for distributed tracing)
        problemDetails.Extensions.Should().ContainKey("traceId");
        problemDetails.Extensions["traceId"].Should().NotBeNull();

        // Verify timestamp field is present
        problemDetails.Extensions.Should().ContainKey("timestamp");
        problemDetails.Extensions["timestamp"].Should().NotBeNull();

        // Verify validation errors for Date field
        problemDetails.Errors.Should().ContainKey("Date");
        problemDetails.Errors["Date"].Should().Contain(error => error.Contains("required"));
    }
}
