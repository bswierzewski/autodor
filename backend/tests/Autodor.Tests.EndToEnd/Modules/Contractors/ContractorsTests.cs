using System.Net;
using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Tests.E2E.Core;
using Autodor.Tests.E2E.Core.Extensions;
using Autodor.Tests.E2E.Core.Factories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.E2E.Modules.Contractors;

/// <summary>
/// End-to-end tests for contractor management functionality including creation, retrieval, and deletion operations.
/// </summary>
public class ContractorsTests(TestWebApplicationFactory factory) : TestBase(factory)
{
    protected override Task OnInitializeAsync()
    {
        Client.WithBearerToken(TestJwtTokens.Default);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetContractors_ShouldReturnSuccess()
    {
        var response = await Client.GetAsync("/api/contractors");

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CreateContractor_WithValidData_ShouldCreateAndReturnContractor()
    {
        // Arrange
        var command = new CreateContractorCommand("Test Company", "1234567890", "Test Street 123", "Test City", "12-345", "test@example.com");

        // Act
        var response = await Client.PostJsonAsync("/api/contractors", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify in database using read context
        var readContext = Services.GetRequiredService<IContractorsReadDbContext>();
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
        var mediator = Services.GetRequiredService<IMediator>();

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
        var response = await Client.GetAsync("/api/contractors");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify database count using read context
        var readContext = Services.GetRequiredService<IContractorsReadDbContext>();
        var contractorsCount = await readContext.Contractors.CountAsync();
        contractorsCount.Should().Be(3);
    }

    [Fact]
    public async Task GetContractorById_WithExistingId_ShouldReturnContractor()
    {
        // Arrange
        var mediator = Services.GetRequiredService<IMediator>();

        var contractorId = await mediator.Send(new CreateContractorCommand(
            "Test Company",
            "5555555555",
            "Test Street 789",
            "Test City",
            "55-555",
            "testget@example.com"
        ));

        // Act
        var response = await Client.GetAsync($"/api/contractors/{contractorId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify contractor exists in database using read context
        var readContext = Services.GetRequiredService<IContractorsReadDbContext>();
        var contractor = await readContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(contractorId));
        contractor.Should().NotBeNull();
        contractor!.Name.Should().Be("Test Company");
    }

    [Fact]
    public async Task DeleteContractor_WithExistingId_ShouldRemoveFromDatabase()
    {
        // Arrange & Act & Assert - No scope test with direct Services access
        var mediator = Services.GetRequiredService<IMediator>();
        var readContext = Services.GetRequiredService<IContractorsReadDbContext>();

        var contractorId = await mediator.Send(new CreateContractorCommand(
            "To Delete Company",
            "7777777777",
            "Delete Street 123",
            "Delete City",
            "77-777",
            "delete@example.com"
        ));

        // Verify contractor exists before deletion using read context
        var contractorBeforeDelete = await readContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(contractorId));
        contractorBeforeDelete.Should().NotBeNull();

        // Act - Delete via HTTP
        var response = await Client.DeleteAsync($"/api/contractors/{contractorId}");

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