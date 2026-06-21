using Autodor.Modules.Orders.Contracts.Models;
using Autodor.Modules.Orders.Contracts.Queries;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace Autodor.Tests.Integration.Orders.GetOrdersByDateRange;

[Collection(SharedCollection.Name)]
public class GetOrdersByDateRangeTests(AutodorDatabaseFixture databaseFixture) : IntegrationTestBase<Program>(databaseFixture)
{
    [Fact(Skip = "Manual test - requires real API connection and valid date range")]
    public async Task GetOrdersByDateRange_WhenOrdersExist_ShouldReturnOrders()
    {
        // Arrange - Replace with actual date range from test environment
        var query = new GetOrdersByDateRangeQuery(
            new DateTime(2026, 2, 5),
            new DateTime(2026, 2, 7));

        var bus = Host.Services.GetRequiredService<IMessageBus>();

        // Act
        var orders = (await bus.InvokeAsync<IEnumerable<OrderDto>>(query, CancellationToken.None)).ToList();

        // Assert
        orders.Should().NotBeNull();
        orders.Should().OnlyContain(order => order.Items.Any());
    }
}
