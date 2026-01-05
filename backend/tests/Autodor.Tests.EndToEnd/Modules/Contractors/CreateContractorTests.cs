using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using BuildingBlocks.Tests.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Autodor.Tests.EndToEnd.Modules.Contractors;

[Collection("Autodor")]
public class CreateContractorTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{
    [Fact]
    public async Task CreateContractor_WithValidData_ShouldCreateAndReturnContractor()
    {
        // Arrange
        var command = new CreateContractorCommand("Test Company", "1234567890", "Test Street 123", "Test City", "12-345", "test@example.com");

        // Act
        var response = await Context.Client.PostJsonAsync("/api/contractors", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify in database
        var readContext = Context.GetRequiredService<IContractorsDbContext>();
        var contractorsCount = await readContext.Contractors.CountAsync();
        contractorsCount.Should().Be(1);

        var contractor = await readContext.Contractors.FirstAsync();
        contractor.Name.Should().Be("Test Company");
        contractor.NIP.Value.Should().Be("1234567890");
    }
}
