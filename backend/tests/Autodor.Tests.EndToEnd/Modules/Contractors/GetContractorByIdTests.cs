using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Tests.EndToEnd.Modules.Contractors;

[Collection("Autodor")]
public class GetContractorByIdTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{
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
}
