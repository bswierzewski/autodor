using System.Net;
using Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;
using Autodor.Shared.Contracts.Contractors;
using Autodor.Shared.Contracts.Contractors.Dtos;
using Autodor.Shared.Contracts.Orders;
using Autodor.Shared.Contracts.Orders.Dtos;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;
using Moq;
using Shared.Infrastructure.Tests.Core;
using Shared.Infrastructure.Tests.Extensions.Http;
using Shared.Infrastructure.Tests.Extensions.Services;

namespace Autodor.Tests.EndToEnd.Modules.Invoicing;

/// <summary>
/// Integration tests for InFakt invoice creation with real InFakt API.
/// Uses mocked module APIs (Contractors, Orders, Products) but real InFakt service.
/// </summary>
[Collection("Autodor")]
public class InFaktIntegrationTests(AutodorSharedFixture shared) : IAsyncLifetime
{
    private TestContext _context = null!;

    // Mocks for cross-module APIs
    private readonly Mock<IContractorsAPI> _mockContractorsApi = new();
    private readonly Mock<IOrdersAPI> _mockOrdersApi = new();
    private readonly Mock<IProductsAPI> _mockProductsApi = new();

    // Test data
    private readonly Guid _testContractorId = Guid.NewGuid();
    private readonly DateTime _testDate = DateTime.Now.Date;

    private ContractorDto _testContractor = null!;
    private List<OrderDto> _testOrders = null!;
    private List<ProductDetailsDto> _testProducts = null!;

    public async Task InitializeAsync()
    {
        InitializeTestData();
        SetupMockBehaviors();

        // Create test context with mocks injected
        _context = await TestContext.CreateBuilder<Program>()
            .WithContainer(shared.Container)
            .WithServices((services, _) =>
            {
                // Replace cross-module APIs with mocks
                services.ReplaceInstance(_mockContractorsApi.Object);
                services.ReplaceInstance(_mockOrdersApi.Object);
                services.ReplaceInstance(_mockProductsApi.Object);
            })
            .BuildAsync();

        await _context.ResetDatabaseAsync();

        // Get token using shared provider (has built-in cache)
        var token = await shared.TokenProvider.GetTokenAsync(shared.TestUser.Email, shared.TestUser.Password);
        _context.Client.WithBearerToken(token);
    }

    public async Task DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }
    }

    private void InitializeTestData()
    {
        _testContractor = new ContractorDto(
            Id: _testContractorId,
            NIP: "7579750973",
            Name: "Autodor Test Company",
            Street: "Testowa 123",
            City: "Katowice",
            ZipCode: "40-001",
            Email: "test@autodor.pl"
        );

        _testOrders =
        [
            new(
                Id: "TEST-ORDER-001",
                Number: "TO-001",
                EntryDate: _testDate,
                Contractor: new OrderContractorDto("TEST-CONTR-001", "Autodor Test Company"),
                Items: [
                    new OrderItemDto("TEST-ORDER-001", "TEST-PROD-001", 2, 150.00m, 0.23m),
                    new OrderItemDto("TEST-ORDER-001", "TEST-PROD-002", 1, 300.00m, 0.23m),
                    new OrderItemDto("TEST-ORDER-001", "TEST-PROD-003", 6, 50.00m, 0.23m),
                ]
            )
        ];

        _testProducts =
        [
            new("TEST-PROD-001", "Usługa testowa A", "TEST001"),
            new("TEST-PROD-002", "Usługa testowa B", "TEST002")
        ];
    }

    private void SetupMockBehaviors()
    {
        _mockContractorsApi.Setup(x => x.GetContractorByIdAsync(_testContractorId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(_testContractor);

        _mockOrdersApi.Setup(x => x.GetOrdersByDatesAsync(It.IsAny<IEnumerable<DateTime>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(_testOrders);

        _mockProductsApi.Setup(x => x.GetProductsByNumbersAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(_testProducts);
    }

    [Fact]
    public async Task CreateInvoice_WithRealInFaktAPI_ShouldCreateInvoiceSuccessfully()
    {
        // Arrange
        var command = new CreateInvoiceCommand(
            InvoiceNumber: null, // Let InFakt auto-generate
            SaleDate: _testDate,
            IssueDate: _testDate,
            Dates: [_testDate],
            OrderIds: ["TEST-ORDER-001"],
            ContractorId: _testContractorId
        );

        // Act
        var response = await _context.Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateInvoice_WithRealInFaktAPI_ShouldHandleErrorsGracefully()
    {
        // Arrange - Use invalid contractor data to trigger InFakt validation error
        var invalidContractorId = Guid.NewGuid();
        var invalidContractor = new ContractorDto(
            Id: invalidContractorId,
            NIP: "INVALID-NIP", // Invalid NIP format
            Name: "",           // Empty name
            Street: "",
            City: "",
            ZipCode: "INVALID",
            Email: "invalid-email"
        );

        // Override mock for this specific test
        _mockContractorsApi.Setup(x => x.GetContractorByIdAsync(invalidContractorId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(invalidContractor);

        var command = new CreateInvoiceCommand(
            InvoiceNumber: 99999,
            SaleDate: _testDate,
            IssueDate: _testDate,
            Dates: [_testDate],
            OrderIds: ["TEST-ORDER-001"],
            ContractorId: invalidContractorId
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _context.Client.PostJsonAsync("/api/invoicing/create", command));

        exception.Message.Should().Contain("Failed to create or update contractor in InFakt");
    }
}
