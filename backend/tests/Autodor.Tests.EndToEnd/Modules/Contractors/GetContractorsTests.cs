using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using MediatR;

namespace Autodor.Tests.EndToEnd.Modules.Contractors;

[Collection("Autodor")]
public class GetContractorsTests(AutodorSharedFixture fixture) : AutodorTestBase(fixture)
{
    [Fact]
    public async Task GetContractors_ShouldReturnSuccess()
    {
        // Act
        var response = await Context.Client.GetAsync("/api/contractors");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
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
    }
}
