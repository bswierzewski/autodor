using Autodor.Modules.Orders.Domain.Entities;
using BuildingBlocks.Abstractions.Abstractions;
using BuildingBlocks.Tests.Core;
using BuildingBlocks.Tests.Extensions.Http;
using BuildingBlocks.Tests.Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.EndToEnd.Modules.Orders;

[Collection("Autodor")]
public class OrdersTests(AutodorSharedFixture fixture) : IAsyncLifetime
{
    private readonly AutodorSharedFixture _fixture = fixture;
    private TestContext _context = null!;

    public async Task InitializeAsync()
    {
        _context = await TestContext.CreateBuilder<Program>()
            .WithContainer(_fixture.Container)
            .WithoutModuleInitialization()
            .WithServices((services, _) =>
            {
                // Register test authentication handler for bypassing authorization in tests
                services.AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
                    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions,
                        TestAuthenticationHandler>(
                        TestAuthenticationHandler.AuthenticationScheme, null);

                // Replace IUserContext with test implementation
                services.AddScoped<IUserContext, TestUserContext>();
            })
            .BuildAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetOrdersByDate_ShouldReturnOrderCollection()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);

        // Act
        var response = await _context.Client.GetAsync($"/api/orders/by-date?date={date:yyyy-MM-dd}");
        var orders = await response.ReadAsJsonAsync<List<Order>>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        orders.Should().NotBeNull();
        orders.Should().HaveCount(2);
        orders.First().Id.Should().Be("ORDER-001");
    }

    [Fact]
    public async Task GetOrdersByDateRange_ShouldReturnOrderCollection()
    {
        // Arrange
        var dateFrom = new DateTime(2024, 1, 1);
        var dateTo = new DateTime(2024, 1, 31);

        // Act
        var response = await _context.Client.GetAsync($"/api/orders/by-date-range?dateFrom={dateFrom:yyyy-MM-dd}&dateTo={dateTo:yyyy-MM-dd}");
        var orders = await response.ReadAsJsonAsync<List<Order>>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        orders.Should().NotBeNull();
        orders.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetOrderById_WithValidOrderId_ShouldReturnOrder()
    {
        // Arrange
        var orderId = "ORDER-001";

        // Act
        var response = await _context.Client.GetAsync($"/api/orders/by-id/{orderId}");
        var order = await response.ReadAsJsonAsync<Order>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        order.Should().NotBeNull();
        order.Id.Should().Be("ORDER-001");
        order.Number.Should().Be("POL-2024-001");
        order.Contractor.Name.Should().Be("John Doe");
    }
}
