using Autodor.Modules.Orders.Contracts.Models;
using Autodor.Modules.Orders.Contracts.Queries;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace Autodor.Tests.Integration.Orders.GetOrdersByDates;

public class GetOrdersByDatesTests(AutodorDatabaseFixture databaseFixture, HostFixture<Program> hostFixture) : IntegrationTest(databaseFixture, hostFixture)
{
    [Fact(Skip = "Manual test - requires real API connection and valid order dates")]
    public async Task GetOrdersByDates_WhenOrdersExist_ShouldReturnOrders()
    {
        // Arrange - Replace with actual dates from test environment
        var query = new GetOrdersByDatesQuery([
            new DateTime(2026, 2, 5),
            new DateTime(2026, 2, 7)
        ]);

        var bus = Host.Services.GetRequiredService<IMessageBus>();

        // Act
        var orders = (await bus.InvokeAsync<IEnumerable<OrderDto>>(query, CancellationToken.None)).ToList();

        // Assert
        orders.Should().NotBeNull();
        orders.Should().OnlyContain(order => order.Items.Any());
    }
}
