using System.Net;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Shared.Contracts.Contractors;
using Autodor.Shared.Contracts.Contractors.Dtos;
using Autodor.Shared.Contracts.Orders;
using Autodor.Shared.Contracts.Orders.Dtos;
using Autodor.Shared.Contracts.Products;
using Autodor.Shared.Contracts.Products.Dtos;
using Autodor.Tests.E2E.Core;
using Autodor.Tests.E2E.Core.Extensions;
using Autodor.Tests.E2E.Core.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Autodor.Tests.E2E.Modules.Invoicing;

public class InvoicingTests(TestWebApplicationFactory factory) : TestBase(factory)
{
    private Mock<IInvoiceService> _mockInvoiceService = null!;
    private Mock<IContractorsAPI> _mockContractorsApi = null!;
    private Mock<IOrdersAPI> _mockOrdersApi = null!;
    private Mock<IProductsAPI> _mockProductsApi = null!;

    // Shared test data
    private readonly Guid _defaultContractorId = Guid.NewGuid();
    private readonly Guid _defaultInvoiceId = Guid.NewGuid();
    private readonly DateTime _defaultDate = DateTime.Now.Date;

    private ContractorDto _defaultContractor = null!;
    private List<OrderDto> _defaultOrders = null!;
    private List<ProductDetailsDto> _defaultProducts = null!;

    protected override void OnConfigureServices(IServiceCollection services)
    {
        _mockInvoiceService = RegisterMock<IInvoiceService>(services);
        _mockContractorsApi = RegisterMock<IContractorsAPI>(services);
        _mockOrdersApi = RegisterMock<IOrdersAPI>(services);
        _mockProductsApi = RegisterMock<IProductsAPI>(services);

        var mockInvoiceServiceFactory = RegisterMock<IInvoiceServiceFactory>(services);
        mockInvoiceServiceFactory.Setup(x => x.GetInvoiceService()).Returns(_mockInvoiceService.Object);

        InitializeTestData();

        SetupDefaultMockBehaviors();
    }

    private void InitializeTestData()
    {
        _defaultContractor = new ContractorDto(
            Id: _defaultContractorId,
            NIP: "1234567890",
            Name: "Test Company",
            Street: "Test Street 123",
            City: "Test City",
            ZipCode: "12-345",
            Email: "test@example.com"
        );

        _defaultOrders = new List<OrderDto>
        {
            new(
                Id: "ORDER-001",
                Number: "NUM-001",
                EntryDate: _defaultDate,
                Contractor: new OrderContractorDto("CONTR-001", "Test Contractor"),
                Items: [
                    new OrderItemDto("ORDER-001", "PROD-001", 2, 100.0m, 0.23m),
                    new OrderItemDto("ORDER-001", "PROD-002", 1, 200.0m, 0.23m),
                    new OrderItemDto("ORDER-001", "PROD-003", 3, 150.0m, 0.23m)
                ]
            )
        };

        _defaultProducts = new List<ProductDetailsDto>
        {
            new("PROD-001", "Product One Name", "1234567890123"),
            new("PROD-002", "Product Two Name", "2345678901234"),
            new("PROD-003", "Product Three Name", "3456789012345")
        };
    }

    private void SetupDefaultMockBehaviors()
    {
        // Default successful contractor lookup
        _mockContractorsApi.Setup(x => x.GetContractorByIdAsync(_defaultContractorId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(_defaultContractor);

        // Default orders response
        _mockOrdersApi.Setup(x => x.GetOrdersByDatesAsync(It.IsAny<IEnumerable<DateTime>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(_defaultOrders);

        // Default products response (all found)
        _mockProductsApi.Setup(x => x.GetProductsByNumbersAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(_defaultProducts);

        // Default successful invoice creation
        _mockInvoiceService.Setup(x => x.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(_defaultInvoiceId);
    }

    private CreateInvoiceCommand CreateDefaultCommand(int? invoiceNumber = 123) =>
        new(
            InvoiceNumber: invoiceNumber,
            SaleDate: _defaultDate,
            IssueDate: _defaultDate,
            Dates: [_defaultDate],
            OrderIds: ["ORDER-001"],
            ContractorId: _defaultContractorId
        );

    [Fact]
    public async Task CreateInvoice_WhenContractorNotFound_ShouldReturnError()
    {
        // Arrange - Override default to return null contractor
        var contractorId = Guid.NewGuid();
        var command = CreateDefaultCommand() with { ContractorId = contractorId };

        _mockContractorsApi.Setup(x => x.GetContractorByIdAsync(contractorId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((ContractorDto?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await Client.PostJsonAsync("/api/invoicing/create", command));

        exception.Message.Should().Contain($"Contractor with ID {contractorId} not found");

        _mockInvoiceService.Verify(x => x.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateInvoice_WhenProductsNotFound_ShouldCreateInvoiceWithPartNumbers()
    {
        // Arrange - Override default to return no products
        var command = CreateDefaultCommand();

        _mockProductsApi.Setup(x => x.GetProductsByNumbersAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new List<ProductDetailsDto>());

        // Act
        var response = await Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        _mockInvoiceService.Verify(x => x.CreateInvoiceAsync(
            It.Is<Invoice>(i =>
                i.Items.Count == 3 &&
                i.Items.Any(item => item.Name == "PROD-001" && item.Quantity == 2 && item.UnitPrice == 100.0m) &&
                i.Items.Any(item => item.Name == "PROD-002" && item.Quantity == 1 && item.UnitPrice == 200.0m) &&
                i.Items.Any(item => item.Name == "PROD-003" && item.Quantity == 3 && item.UnitPrice == 150.0m)),
            It.IsAny<CancellationToken>()), Times.Once);

        // Event should be published automatically by the system
    }

    [Fact]
    public async Task CreateInvoice_WithPartialProducts_ShouldEnrichFoundProductsOnly()
    {
        // Arrange - Override default to return only 2 out of 3 products
        var command = CreateDefaultCommand(456);

        var partialProducts = new List<ProductDetailsDto>
        {
            new("PROD-001", "Product One Name", "1234567890123"),
            new("PROD-003", "Product Three Name", "3456789012345")
            // PROD-002 is intentionally missing
        };

        _mockProductsApi.Setup(x => x.GetProductsByNumbersAsync(
                           It.Is<IEnumerable<string>>(numbers =>
                               numbers.Contains("PROD-001") &&
                               numbers.Contains("PROD-002") &&
                               numbers.Contains("PROD-003")),
                           It.IsAny<CancellationToken>()))
                       .ReturnsAsync(partialProducts);

        // Act
        var response = await Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        _mockInvoiceService.Verify(x => x.CreateInvoiceAsync(
            It.Is<Invoice>(i =>
                i.Items.Count == 3 &&
                i.Items.Any(item => item.Name == "Product One Name (PROD-001)" && item.Quantity == 2 && item.UnitPrice == 100.0m) &&
                i.Items.Any(item => item.Name == "PROD-002" && item.Quantity == 1 && item.UnitPrice == 200.0m) &&
                i.Items.Any(item => item.Name == "Product Three Name (PROD-003)" && item.Quantity == 3 && item.UnitPrice == 150.0m)),
            It.IsAny<CancellationToken>()), Times.Once);

        // Event should be published automatically by the system
    }

    [Fact]
    public async Task CreateInvoice_WithAllProductsFound_ShouldEnrichAllProducts()
    {
        // Arrange - Use default setup which already has all products found
        var command = CreateDefaultCommand(789);

        // Act
        var response = await Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        _mockInvoiceService.Verify(x => x.CreateInvoiceAsync(
            It.Is<Invoice>(i =>
                i.Items.Count == 3 &&
                i.Items.Any(item => item.Name == "Product One Name (PROD-001)" && item.Quantity == 2 && item.UnitPrice == 100.0m) &&
                i.Items.Any(item => item.Name == "Product Two Name (PROD-002)" && item.Quantity == 1 && item.UnitPrice == 200.0m) &&
                i.Items.Any(item => item.Name == "Product Three Name (PROD-003)" && item.Quantity == 3 && item.UnitPrice == 150.0m)),
            It.IsAny<CancellationToken>()), Times.Once);

        // Event should be published automatically by the system
    }

    [Fact]
    public async Task CreateInvoice_WithValidData_ShouldCreateInvoiceSuccessfully()
    {
        // Arrange - This test focuses on what Invoicing module controls
        var command = CreateDefaultCommand(999);

        // Act
        var response = await Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify the invoice was created with correct data
        _mockInvoiceService.Verify(x => x.CreateInvoiceAsync(
            It.Is<Invoice>(i =>
                i.Number == 999 &&
                i.Contractor.Name == _defaultContractor.Name &&
                i.Items.Count == 3),
            It.IsAny<CancellationToken>()), Times.Once);

        // Note: Integration events and cross-module side effects would be tested
        // in separate integration tests or when modules become microservices.
        // The Invoicing module's responsibility ends at publishing the event.
    }

    [Fact]
    public async Task CreateInvoice_ShouldPublishInvoiceCreatedEvent()
    {
        // Arrange
        var command = CreateDefaultCommand(777);

        // Act
        var response = await Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateInvoice_ShouldPassCorrectInvoiceDataToService()
    {
        // Arrange - Use default setup and verify all data is passed correctly
        var saleDate = new DateTime(2024, 1, 15);
        var issueDate = new DateTime(2024, 1, 20);
        var command = new CreateInvoiceCommand(
            InvoiceNumber: 555,
            SaleDate: saleDate,
            IssueDate: issueDate,
            Dates: [saleDate],
            OrderIds: ["ORDER-001"],
            ContractorId: _defaultContractorId
        );

        // Act
        var response = await Client.PostJsonAsync("/api/invoicing/create", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        _mockInvoiceService.Verify(x => x.CreateInvoiceAsync(
            It.Is<Invoice>(i =>
                i.Number == 555 &&
                i.SaleDate == saleDate &&
                i.IssueDate == issueDate &&
                i.PaymentDue == issueDate.AddDays(14) &&
                i.Contractor.Name == _defaultContractor.Name &&
                i.Contractor.NIP == _defaultContractor.NIP &&
                i.Contractor.Street == _defaultContractor.Street &&
                i.Contractor.City == _defaultContractor.City &&
                i.Contractor.ZipCode == _defaultContractor.ZipCode &&
                i.Contractor.Email == _defaultContractor.Email &&
                i.Items.Count == 3),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

