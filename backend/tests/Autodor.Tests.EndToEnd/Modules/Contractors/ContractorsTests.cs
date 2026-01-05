using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using BuildingBlocks.Tests.Extensions.Http;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Autodor.Tests.EndToEnd.Modules.Contractors;

[Collection("Autodor")]
public class ContractorsTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{
    // No need for InitializeAsync/DisposeAsync - handled by base class
    // Override ConfigureServices if you need to register custom services for all tests in this class

    [Fact]
    public async Task GetContractors_ShouldReturnSuccess()
    {
        var response = await Context.Client.GetAsync("/api/contractors");

        response.IsSuccessStatusCode.Should().BeTrue();
    }

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

    [Fact]
    public async Task GetContractors_WithMultipleContractors_ShouldReturnCorrectCount()
    {
        // Arrange - Create multiple contractors
        var mediator = Context.GetRequiredService<IMediator>();

        var commands = new[]
        {
            new CreateContractorCommand("Company 1", "1111111111", "Street 1", "City 1", "11-111", "test1@example.com"),
            new CreateContractorCommand("Company 2", "2222222222", "Street 2", "City 2", "22-222", "test2@example.com"),
            new CreateContractorCommand("Company 3", "3333333333", "Street 3", "City 3", "33-333", "test3@example.com")
        };

        foreach (var command in commands)
        {
            await mediator.Send(command);
        }

        // Act
        var response = await Context.Client.GetAsync("/api/contractors");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify database count
        var readContext = Context.GetRequiredService<IContractorsDbContext>();
        var contractorsCount = await readContext.Contractors.CountAsync();
        contractorsCount.Should().Be(3);
    }

    [Fact]
    public async Task GetContractorById_WithExistingId_ShouldReturnContractor()
    {
        // Arrange
        var mediator = Context.GetRequiredService<IMediator>();

        var result = await mediator.Send(new CreateContractorCommand(
            "Test Company",
            "5555555555",
            "Test Street 789",
            "Test City",
            "55-555",
            "testget@example.com"
        ));
        var contractorId = result.Value;

        // Act
        var response = await Context.Client.GetAsync($"/api/contractors/{contractorId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify contractor exists in database
        var readContext = Context.GetRequiredService<IContractorsDbContext>();
        var contractor = await readContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(contractorId));
        contractor.Should().NotBeNull();
        contractor!.Name.Should().Be("Test Company");
    }

    [Fact]
    public async Task DeleteContractor_WithExistingId_ShouldRemoveFromDatabase()
    {
        // Arrange
        var mediator = Context.GetRequiredService<IMediator>();
        var readContext = Context.GetRequiredService<IContractorsDbContext>();

        var result = await mediator.Send(new CreateContractorCommand(
            "To Delete Company",
            "7777777777",
            "Delete Street 123",
            "Delete City",
            "77-777",
            "delete@example.com"
        ));
        var contractorId = result.Value;

        // Verify contractor exists before deletion
        var contractorBeforeDelete = await readContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(contractorId));
        contractorBeforeDelete.Should().NotBeNull();

        // Act - Delete via HTTP
        var response = await Context.Client.DeleteAsync($"/api/contractors/{contractorId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify contractor no longer exists - fresh query due to AsNoTracking
        var contractorAfterDelete = await readContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(contractorId));
        contractorAfterDelete.Should().BeNull();

        var totalCount = await readContext.Contractors.CountAsync();
        totalCount.Should().Be(0);
    }
}