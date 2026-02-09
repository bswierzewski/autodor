using System.Collections.Frozen;
using System.Net;
using Autodor.API.IntegrationTests.Shared;
using Autodor.Modules.Contractors.Contracts.Abstractions;
using Autodor.Modules.Contractors.Contracts.Models;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Models;
using BuildingBlocks.IntegrationTests;
using BuildingBlocks.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit.Abstractions;

namespace Autodor.API.IntegrationTests.Modules.Orders.GenerateDeliveryNote;

[Collection(SharedCollection.Name)]
public class GenerateDeliveryNoteTests(DatabaseFixture databaseFixture, ITestOutputHelper output)
    : TestBase<Program>(databaseFixture), IAsyncLifetime
{
    private readonly Mock<IContractorsModuleApi> _contractorsApiMock = new();
    private readonly Mock<IProductsClient> _productsClientMock = new();

    protected override Task SeedDataAsync()
    {
        // Setup mock products client with test data
        var testProducts = new Dictionary<string, Product>
        {
            ["3205959"] = new Product { Number = "3205959", Name = "Tarcza hamulcowa przednia" },
            ["2008554E"] = new Product { Number = "2008554E", Name = "Klocki hamulcowe tylne" },
            ["3202RWT2"] = new Product { Number = "3202RWT2", Name = "Zestaw sprzęgła kompletny" }
        }.ToFrozenDictionary();

        _productsClientMock
            .Setup(x => x.GetProductsAsync())
            .ReturnsAsync(testProducts);

        return Task.CompletedTask;
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // Replace IContractorsModuleApi with mock
        services.RemoveAll<IContractorsModuleApi>();
        services.AddSingleton(_contractorsApiMock.Object);

        // Replace IProductsClient with mock
        services.RemoveAll<IProductsClient>();
        services.AddSingleton(_productsClientMock.Object);

        // Setup mock to return test contractor
        var testContractor = new ContractorDto(
            Id: Guid.NewGuid(),
            Name: "Test Contractor Sp. z o.o.",
            NIP: "1234567890",
            Street: "ul. Testowa 123",
            City: "Warszawa",
            ZipCode: "00-001",
            Email: "test@contractor.pl"
        );

        _contractorsApiMock
            .Setup(x => x.GetContractorByNipAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testContractor);
    }

    [Fact(Skip = "Manual test - requires real API connection")]
    public async Task GenerateDeliveryNote_ShouldGenerateAndSavePdfFile()
    {       
        // Arrange - Replace with actual order ID and date from test environment
        var orderId = "3ff0615c-b902-f111-95f5-00155d0b7aef";
        var orderDate = new DateTime(2026, 2, 5);

        // Act
        var result = await AlbaHost.Scenario(s =>
        {
            s.Post.Json(new
            {
                OrderId = orderId,
                Date = orderDate
            }).ToUrl("/orders/delivery-note");

            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        // Get PDF bytes from response
        var response = result.Context.Response;
        using var memoryStream = new MemoryStream();
        await response.Body.CopyToAsync(memoryStream);
        var pdfBytes = memoryStream.ToArray();

        // Verify PDF was generated
        Assert.NotNull(pdfBytes);
        Assert.NotEmpty(pdfBytes);

        // Save PDF to file for manual inspection
        var outputPath = Path.Combine(Path.GetTempPath(), $"DeliveryNote_{orderId}.pdf");
        await File.WriteAllBytesAsync(outputPath, pdfBytes);

        output.WriteLine($"Generated PDF size: {pdfBytes.Length} bytes");
        output.WriteLine($"PDF saved to: {outputPath}");
        output.WriteLine($"Content-Type: {response.ContentType}");
    }

    [Fact(Skip = "Manual test - requires real API connection")]
    public async Task GenerateDeliveryNote_WhenOrderNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = "non-existent-order-id";
        var orderDate = new DateTime(2026, 2, 5);

        // Act && Assert
        await AlbaHost.Scenario(s =>
        {
            s.Post.Json(new
            {
                OrderId = orderId,
                Date = orderDate
            }).ToUrl("/orders/delivery-note");

            s.StatusCodeShouldBe(HttpStatusCode.NotFound);
        }).PrintBody(output);
    }
}
