using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Models;
using Autodor.Tests.Integration.Shared;
using BuildingBlocks.Tests.Integration.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Frozen;
using System.Net;

namespace Autodor.Tests.Integration.Modules.Orders.GenerateDeliveryNote;

[Collection(GenerateDeliveryNoteCollection.Name)]
public class GenerateDeliveryNoteTests(GenerateDeliveryNoteEnvironment Environment, ITestOutputHelper output)
    : IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        await Environment.ResetDatabaseAsync();
        await SeedDataAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private async Task SeedDataAsync()
    {
        // Setup mock products client with test data
        var testProducts = new Dictionary<string, Product>
        {
            ["3205959"] = new Product { Number = "3205959", Name = "Tarcza hamulcowa przednia" },
            ["2008554E"] = new Product { Number = "2008554E", Name = "Klocki hamulcowe tylne" },
            ["3202RWT2"] = new Product { Number = "3202RWT2", Name = "Zestaw sprzęgła kompletny" }
        }.ToFrozenDictionary();

        Environment.ProductsClientMock
            .Setup(x => x.GetProductsAsync())
            .ReturnsAsync(testProducts);

        // Seed test contractor in database
        await using var scope = Environment.Host.Services.CreateAsyncScope();
        var contractorsDb = scope.ServiceProvider.GetRequiredService<ContractorsDbContext>();

        var contractor = new Contractor(
            new ContractorId(Guid.NewGuid()),
            new TaxId("6961872069"),
            "Test Contractor Sp. z o.o.",
            new Address("ul. Testowa 123", "Warszawa", "00-001"),
            new Email("test@contractor.pl")
        );

        contractorsDb.Contractors.Add(contractor);
        await contractorsDb.SaveChangesAsync();
    }

    [Fact]
    public async Task GenerateDeliveryNote_ShouldGenerateAndSavePdfFile()
    {       
        // Arrange - Replace with actual order ID and date from test environment
        var orderId = "3ff0615c-b902-f111-95f5-00155d0b7aef";
        var orderDate = new DateTime(2026, 2, 5);

        // Act
        var result = await Environment.Host.Scenario(s =>
        {
            s.Post.Json(new
            {
                OrderId = orderId,
                Date = orderDate
            }).ToUrl("/delivery-notes");

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
        await Environment.Host.Scenario(s =>
        {
            s.Post.Json(new
            {
                OrderId = orderId,
                Date = orderDate
            }).ToUrl("/delivery-notes");

            s.StatusCodeShouldBe(HttpStatusCode.NotFound);
        }).PrintBody(output);
    }
}
