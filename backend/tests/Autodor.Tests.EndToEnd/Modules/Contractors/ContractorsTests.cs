using Autodor.Tests.E2E.Core;
using Autodor.Tests.E2E.Core.Factories;

namespace Autodor.Tests.E2E.Modules.Contractors;

public class ContractorsTests(TestWebApplicationFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task GetContractors_ShouldReturnSuccess()
    {
        var response = await Client.GetAsync("/api/contractors");

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}