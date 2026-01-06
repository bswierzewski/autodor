using BuildingBlocks.Tests.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.EndToEnd;

public abstract class AutodorTestBase(AutodorSharedFixture fixture) : IAsyncLifetime
{
    protected TestContext<Program> Context = null!;

    protected virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Override in derived classes to configure custom services
    }

    public virtual async Task InitializeAsync()
    {
        Context = new TestContext<Program>(fixture.Container)
            .WithTestAuthentication()
            .WithServices(ConfigureServices);

        await Context.InitializeAsync();
    }

    public virtual async Task DisposeAsync()
    {
        if (Context != null)
        {
            await Context.DisposeAsync();
        }
    }
}
