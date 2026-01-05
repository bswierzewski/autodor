using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Autodor.Tests.EndToEnd.Modules.Contractors;

[Collection("Autodor")]
public class DeleteContractorTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{
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
