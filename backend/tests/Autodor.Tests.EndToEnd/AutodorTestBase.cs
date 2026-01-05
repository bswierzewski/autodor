using BuildingBlocks.Tests.Builders;
using BuildingBlocks.Tests.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Tests.EndToEnd;

public abstract class AutodorTestBase(AutodorSharedFixture fixture) : IAsyncLifetime
{
    protected TestContext Context = null!;

    protected virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Override in derived classes to configure custom services
    }

    protected virtual TestContextBuilder<Program> ConfigureBuilder(TestContextBuilder<Program> builder)
    {
        return builder;
    }

    public virtual async Task InitializeAsync()
    {
        var builder = TestContext.CreateBuilder<Program>()
            .WithContainer(fixture.Container)
            .WithTestAuthentication();

        // Apply services configuration hook
        builder = builder.WithServices(ConfigureServices);

        // Apply builder configuration hook
        builder = ConfigureBuilder(builder);

        Context = await builder.BuildAsync();
        await Context.ResetDatabaseAsync();
    }

    public virtual async Task DisposeAsync()
    {
        if (Context != null)
        {
            await Context.DisposeAsync();
        }
    }
}
