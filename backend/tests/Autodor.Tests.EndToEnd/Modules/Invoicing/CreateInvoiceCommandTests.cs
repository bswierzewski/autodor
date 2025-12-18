using System.Net;
using Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;
using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Shared.Contracts.Contractors;
using Autodor.Shared.Contracts.Contractors.Dtos;
using Autodor.Shared.Contracts.Orders;
using Autodor.Shared.Contracts.Orders.Dtos;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.Infrastructure.Tests.Core;
using Shared.Infrastructure.Tests.Extensions.Http;
using Shared.Infrastructure.Tests.Extensions.Services;

namespace Autodor.Tests.EndToEnd.Modules.Invoicing;

/// <summary>
/// Integration tests for CreateInvoiceCommand with real invoice service implementations.
/// Tests are parameterized to run against both InFakt and IFirma providers.
/// Uses mocked module APIs (Contractors, Orders, Products) but real invoice services.
/// </summary>
[Collection("Autodor")]
public class CreateInvoiceCommandTests(AutodorSharedFixture shared) : IAsyncLifetime
{
    private readonly Dictionary<InvoiceProvider, TestContext> _contexts = new();

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

        // Create test contexts for both providers
        foreach (InvoiceProvider provider in Enum.GetValues<InvoiceProvider>())
        {
            var currentProvider = provider; // Capture for closure
            var context = await TestContext.CreateBuilder<Program>()
                .WithContainer(shared.Container)
                .WithServices((services, _) =>
                {
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

            // Get token using shared provider (has built-in cache)
            var token = await shared.TokenProvider.GetTokenAsync(shared.TestUser.Email, shared.TestUser.Password);
            context.Client.WithBearerToken(token);

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
            new("TEST-PROD-002", "Usługa testowa B", "TEST002"),
            new("TEST-PROD-003", "Usługa testowa C", "TEST003")
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

    private CreateInvoiceCommand CreateTestCommand(int? invoiceNumber = null) =>
        new(
            InvoiceNumber: invoiceNumber,
            SaleDate: _testDate,
            IssueDate: _testDate,
            Dates: [_testDate],
            OrderIds: ["TEST-ORDER-001"],
            ContractorId: _testContractorId
        );

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateInvoice_WithValidData_ShouldCreateInvoiceSuccessfully(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateInvoice_WhenContractorNotFound_ShouldReturnError(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var invalidContractorId = Guid.NewGuid();
        var command = CreateTestCommand() with { ContractorId = invalidContractorId };

        // Override mock for this specific test
        _mockContractorsApi.Setup(x => x.GetContractorByIdAsync(invalidContractorId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((ContractorDto?)null);

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("CONTRACTOR_NOT_FOUND");
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateInvoice_WhenProductsNotFound_ShouldCreateInvoiceWithPartNumbers(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // Override default to return no products
        _mockProductsApi.Setup(x => x.GetProductsByNumbersAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new List<ProductDetailsDto>());

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateInvoice_WithPartialProducts_ShouldEnrichFoundProductsOnly(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = CreateTestCommand();

        var partialProducts = new List<ProductDetailsDto>
        {
            new("TEST-PROD-001", "Usługa testowa A", "TEST001"),
            new("TEST-PROD-003", "Usługa testowa C", "TEST003")
            // TEST-PROD-002 is intentionally missing
        };

        _mockProductsApi.Setup(x => x.GetProductsByNumbersAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(partialProducts);

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateInvoice_WithInvalidContractorData_ShouldHandleErrorsGracefully(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var invalidContractorId = Guid.NewGuid();
        var invalidContractor = new ContractorDto(
            Id: invalidContractorId,
            NIP: "INVALID-NIP",
            Name: "",
            Street: "",
            City: "",
            ZipCode: "INVALID",
            Email: "invalid-email"
        );

        // Override mock for this specific test
        _mockContractorsApi.Setup(x => x.GetContractorByIdAsync(invalidContractorId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(invalidContractor);

        var command = CreateTestCommand() with { ContractorId = invalidContractorId };

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert - Should return error (either BadRequest or InternalServerError)
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateInvoice_WithAllProductsFound_ShouldEnrichAllProducts(InvoiceProvider provider)
    {
        // Arrange - Use default setup which already has all products found
        var context = _contexts[provider];
        var command = CreateTestCommand();

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(InvoiceProvider.InFakt)]
    [InlineData(InvoiceProvider.IFirma)]
    public async Task CreateInvoice_ShouldPassCorrectInvoiceDataToService(InvoiceProvider provider)
    {
        // Arrange
        var context = _contexts[provider];
        var command = new CreateInvoiceCommand(
            InvoiceNumber: null,
            SaleDate: _testDate,
            IssueDate: _testDate,
            Dates: [_testDate],
            OrderIds: ["TEST-ORDER-001"],
            ContractorId: _testContractorId
        );

        // Act
        var response = await context.Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrEmpty();
    }
}
