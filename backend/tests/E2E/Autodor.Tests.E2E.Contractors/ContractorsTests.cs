using Autodor.Tests.E2E.Core;
using Autodor.Tests.E2E.Core.Factories;

namespace Autodor.Tests.E2E.Contractors;

/// <summary>
/// E2E tests for contractors functionality.
/// Inherits from TestBase which provides shared PostgreSQL container and database management.
/// </summary>
public class ContractorsTests(TestWebApplicationFactory factory) : TestBase(factory), IClassFixture<TestWebApplicationFactory>
{
    [Fact]
    public async Task GetContractors_ShouldReturnSuccess()
    {
        // Act
        var response = await Client.GetAsync("/api/contractors");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}