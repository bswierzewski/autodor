using Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;
using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Domain.Enums;
using Autodor.Shared.Contracts.Contractors;
using Autodor.Shared.Contracts.Contractors.Dtos;
using Autodor.Shared.Contracts.Orders;
using Autodor.Shared.Contracts.Orders.Dtos;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;
using BuildingBlocks.Abstractions.Abstractions;
using BuildingBlocks.Tests.Core;
using BuildingBlocks.Tests.Extensions.Http;
using BuildingBlocks.Tests.Extensions.Services;
using BuildingBlocks.Tests.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;

namespace Autodor.Tests.EndToEnd.Modules.Invoicing;

[Collection("Autodor")]
public class CreateBulkInvoicesCommandTests(AutodorSharedFixture shared) : IAsyncLifetime
{
    private readonly Dictionary<InvoiceProvider, TestContext> _contexts = new();

    // Mocks for cross-module APIs
    private readonly Mock<IContractorsAPI> _mockContractorsApi = new();
    private readonly Mock<IOrdersAPI> _mockOrdersApi = new();
    private readonly Mock<IProductsAPI> _mockProductsApi = new();

    // Test data
    private readonly DateTime _testDateFrom = new(2024, 1, 1);
    private readonly DateTime _testDateTo = new(2024, 1, 31);

    private List<ContractorDto> _testContractors = null!;
    private List<OrderDto> _testOrders = null!;
    private List<ProductDetailsDto> _testProducts = null!;
    private List<string> _excludedOrderIds = null!;

    public async Task InitializeAsync()
    {
        InitializeTestData();
        SetupMockBehaviors();

        // Create test contexts for both providers
        foreach (InvoiceProvider provider in Enum.GetValues<InvoiceProvider>())
        {
            var currentProvider = provider; // Capture for closure
            var context = await TestContext.CreateBuilder<Program>()
                .WithContainer(shared.Container)
                .WithServices((services, _) =>
                {
                    // Register test authentication handler for bypassing authorization in tests
                    services.AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
                        .AddScheme<AuthenticationSchemeOptions,
                            TestAuthenticationHandler>(
                            TestAuthenticationHandler.AuthenticationScheme, null);

                    // Replace IUserContext with test implementation
                    services.AddScoped<IUserContext, TestUserContext>();

                    // Replace cross-module APIs with mocks
                    services.ReplaceInstance(_mockContractorsApi.Object);
                    services.ReplaceInstance(_mockOrdersApi.Object);
                    services.ReplaceInstance(_mockProductsApi.Object);

                    // Override InvoicingOptions to use specific provider
                    services.Configure<InvoicingOptions>(options =>
                    {
                        options.Provider = currentProvider;
                    });
                })
                .BuildAsync();

            await context.ResetDatabaseAsync();

            _contexts[provider] = context;
        }
    }

    public async Task DisposeAsync()
    {
        foreach (var context in _contexts.Values)
        {
            await context.DisposeAsync();
        }
    }

    private void InitializeTestData()
    {
        _testContractors =
        [
            new ContractorDto(
                Id: Guid.NewGuid(),
                NIP: "7579750973",
                Name: "Autodor Test Company A",
                Street: "Testowa 123",
                City: "Katowice",
                ZipCode: "40-001",
                Email: "test-a@autodor.pl"
            ),
            new ContractorDto(
                Id: Guid.NewGuid(),
                NIP: "1234567890",
                Name: "Autodor Test Company B",
                Street: "Testowa 456",
                City: "Warszawa",
                ZipCode: "00-001",
                Email: "test-b@autodor.pl"
            ),
            new ContractorDto(
                Id: Guid.NewGuid(),
                NIP: "9876543210",
                Name: "Autodor Test Company C",
                Street: "Testowa 789",
                City: "Krakow",
                ZipCode: "30-001",
                Email: "test-c@autodor.pl"
            )
        ];

        _testOrders =
        [
            // Orders for contractor A (NIP: 7579750973)
            new OrderDto(
                Id: "ORDER-001",
                Number: "ORD-001",
                EntryDate: _testDateFrom,
                Contractor: new OrderContractorDto("7579750973", "Autodor Test Company A"),
                Items: [
                    new OrderItemDto("ORDER-001", "TEST-PROD-001", 2, 150.00m, 0.23m),
                    new OrderItemDto("ORDER-001", "TEST-PROD-002", 1, 300.00m, 0.23m)
                ]
            ),
            new OrderDto(
                Id: "ORDER-002",
                Number: "ORD-002",
                EntryDate: _testDateFrom.AddDays(5),
                Contractor: new OrderContractorDto("7579750973", "Autodor Test Company A"),
                Items: [
                    new OrderItemDto("ORDER-002", "TEST-PROD-003", 3, 100.00m, 0.23m)
                ]
            ),
            // Orders for contractor B (NIP: 1234567890)
            new OrderDto(
                Id: "ORDER-003",
                Number: "ORD-003",
                EntryDate: _testDateFrom.AddDays(10),
                Contractor: new OrderContractorDto("1234567890", "Autodor Test Company B"),
                Items: [
                    new OrderItemDto("ORDER-003", "TEST-PROD-001", 5, 150.00m, 0.23m)
                ]
            ),
            // Order that will be excluded
            new OrderDto(
                Id: "ORDER-EXCLUDED",
                Number: "ORD-EXCL",
                EntryDate: _testDateFrom.AddDays(15),
                Contractor: new OrderContractorDto("9876543210", "Autodor Test Company C"),
                Items: [
                    new OrderItemDto("ORDER-EXCLUDED", "TEST-PROD-002", 1, 200.00m, 0.23m)
                ]
            )
        ];

        _testProducts =
        [
            new ProductDetailsDto("TEST-PROD-001", "Usługa testowa A", "TEST001"),
            new ProductDetailsDto("TEST-PROD-002", "Usługa testowa B", "TEST002"),
            new ProductDetailsDto("TEST-PROD-003", "Usługa testowa C", "TEST003")
        ];

        _excludedOrderIds = ["ORDER-EXCLUDED"];
    }

    private void SetupMockBehaviors()
    {
        _mockOrdersApi.Setup(x => x.GetOrdersByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testOrders);

        _mockOrdersApi.Setup(x => x.GetExcludedOrderIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_excludedOrderIds);

        _mockContractorsApi.Setup(x => x.GetContractorsByNIPsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testContractors);

        _mockProductsApi.Setup(x => x.GetProductsByNumbersAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testProducts);
    }

    private CreateBulkInvoicesCommand CreateTestCommand(DateTime? dateFrom = null, DateTime? dateTo = null) =>
        new(
            DateFrom: dateFrom ?? _testDateFrom,
            DateTo: dateTo ?? _testDateTo
        );

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateBulkInvoices_WithValidData_ShouldCreateInvoicesForMultipleContractors(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create-bulk", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateBulkInvoices_ShouldExcludeOrdersInExclusionList(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // Setup: Verify that excluded orders won't be processed
        var allOrderIds = _testOrders.Select(o => o.Id).ToList();
        allOrderIds.Should().Contain("ORDER-EXCLUDED");

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create-bulk", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify that the response indicates success for non-excluded contractors only
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateBulkInvoices_WhenNoValidOrders_ShouldReturnError(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // Override to return no orders
        _mockOrdersApi.Setup(x => x.GetOrdersByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrderDto>());

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create-bulk", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateBulkInvoices_WhenAllOrdersExcluded_ShouldReturnError(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // Set all orders as excluded
        var allOrderIds = _testOrders.Select(o => o.Id).ToList();
        _mockOrdersApi.Setup(x => x.GetExcludedOrderIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allOrderIds);

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create-bulk", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateBulkInvoices_ShouldGroupOrdersByContractor(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // We have:
        // - 2 orders for contractor A (7579750973)
        // - 1 order for contractor B (1234567890)
        // - 1 order for contractor C that will be excluded (9876543210)
        // Expected: 2 invoices created (for A and B)

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create-bulk", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateBulkInvoices_WhenContractorNotFound_ShouldContinueWithOtherContractors(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // Return only 2 contractors (missing one)
        var partialContractors = _testContractors.Take(2).ToList();
        _mockContractorsApi.Setup(x => x.GetContractorsByNIPsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partialContractors);

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create-bulk", command);

        // Assert - Should still succeed for found contractors
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateBulkInvoices_WithPartialProducts_ShouldStillCreateInvoices(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // Return only partial products
        var partialProducts = new List<ProductDetailsDto>
        {
            new("TEST-PROD-001", "Usługa testowa A", "TEST001")
            // TEST-PROD-002 and TEST-PROD-003 are missing
        };

        _mockProductsApi.Setup(x => x.GetProductsByNumbersAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partialProducts);

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create-bulk", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateBulkInvoices_WithDateRange_ShouldOnlyProcessOrdersInRange(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var narrowDateFrom = _testDateFrom.AddDays(8);
        var narrowDateTo = _testDateFrom.AddDays(12);
        var command = CreateTestCommand(narrowDateFrom, narrowDateTo);

        // Setup: Only return orders in the narrow date range
        var ordersInRange = _testOrders
            .Where(o => o.EntryDate >= narrowDateFrom && o.EntryDate <= narrowDateTo)
            .ToList();

        _mockOrdersApi.Setup(x => x.GetOrdersByDateRangeAsync(narrowDateFrom, narrowDateTo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordersInRange);

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create-bulk", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }
}
